# Phase 8 — Inventory Module

> **Goal**: Implement items, categories, batches, stock levels, barcode scanning, and location management APIs.

---

## Step 8.1 — Category Management

### DTOs

```csharp
public record CategoryDto(int Id, string Name, int? ParentId, string? Description, int ItemCount, List<CategoryDto> Children);
public record CreateCategoryRequest(string Name, int? ParentId, string? Description);
public record UpdateCategoryRequest(string Name, string? Description);
```

### Controller: `CategoriesController`

| Method   | Endpoint               | Policy             | Description                     |
| -------- | ---------------------- | ------------------ | ------------------------------- |
| `GET`    | `/api/categories`      | `CategoriesView`   | List categories (tree or flat)  |
| `GET`    | `/api/categories/{id}` | `CategoriesView`   | Get category with item count    |
| `POST`   | `/api/categories`      | `CategoriesManage` | Create category                 |
| `PUT`    | `/api/categories/{id}` | `CategoriesManage` | Update category                 |
| `DELETE` | `/api/categories/{id}` | `CategoriesManage` | Soft-delete (fail if has items) |

---

## Step 8.2 — Item Management

### DTOs

```csharp
public record ItemDto(int Id, string Sku, string Name, string? CategoryName, string? SupplierName, string UnitType,
    string? Barcode, bool BatchTracked, bool ExpiryTracked, decimal? MinStockLevel, decimal? CostPrice,
    decimal? SellingPrice, decimal TotalStock, string? ImageUrl);
public record ItemDetailDto(ItemDto Item, List<ItemLocationDto> LocationStock, List<BatchDto> Batches);
public record CreateItemRequest(string Sku, string Name, int? CategoryId, int? SupplierId, string UnitType,
    string? Barcode, bool BatchTracked, bool ExpiryTracked, decimal? MinStockLevel, decimal? ReorderQuantity,
    decimal? CostPrice, decimal? SellingPrice, string? StorageConditions);
```

### Controller: `ItemsController`

| Method   | Endpoint                       | Policy        | Description                       |
| -------- | ------------------------------ | ------------- | --------------------------------- |
| `GET`    | `/api/items`                   | `ItemsView`   | Paginated list with search/filter |
| `GET`    | `/api/items/{id}`              | `ItemsView`   | Detail with stock & batches       |
| `POST`   | `/api/items`                   | `ItemsCreate` | Create item                       |
| `PUT`    | `/api/items/{id}`              | `ItemsEdit`   | Update item                       |
| `DELETE` | `/api/items/{id}`              | `ItemsDelete` | Soft-delete                       |
| `GET`    | `/api/items/low-stock`         | `ItemsView`   | Items below min stock level       |
| `GET`    | `/api/items/barcode/{barcode}` | `ItemsView`   | Lookup by barcode                 |
| `POST`   | `/api/items/{id}/image`        | `ItemsEdit`   | Upload image (Azure Blob)         |

### Filtering & Sorting

```
GET /api/items?search=fertilizer&categoryId=5&supplierId=3&lowStockOnly=true&sortBy=name&page=1&pageSize=20
```

### Low Stock Alert Query

```csharp
var lowStock = await _db.Items
    .Where(i => i.MinStockLevel != null)
    .Select(i => new {
        Item = i,
        TotalStock = i.ItemLocations.Sum(il => il.Quantity)
    })
    .Where(x => x.TotalStock <= x.Item.MinStockLevel)
    .ToListAsync();
```

---

## Step 8.3 — Location Management

### Controller: `LocationsController`

| Method   | Endpoint              | Policy            | Description                     |
| -------- | --------------------- | ----------------- | ------------------------------- |
| `GET`    | `/api/locations`      | `LocationsView`   | List all locations              |
| `GET`    | `/api/locations/{id}` | `LocationsView`   | Location with stock summary     |
| `POST`   | `/api/locations`      | `LocationsManage` | Create location                 |
| `PUT`    | `/api/locations/{id}` | `LocationsManage` | Update location                 |
| `DELETE` | `/api/locations/{id}` | `LocationsManage` | Soft-delete (fail if has stock) |

---

## Step 8.4 — Batch / Lot Management

### Controller: `BatchesController`

| Method | Endpoint                      | Policy          | Description               |
| ------ | ----------------------------- | --------------- | ------------------------- |
| `GET`  | `/api/batches`                | `BatchesView`   | List batches (filterable) |
| `GET`  | `/api/items/{itemId}/batches` | `BatchesView`   | Batches for specific item |
| `POST` | `/api/batches`                | `BatchesManage` | Create batch              |
| `GET`  | `/api/batches/expiring`       | `BatchesView`   | Soon-expiring batches     |

### Expiry Alert Query

```csharp
var expiring = await _db.Batches
    .Where(b => b.ExpiryDate != null
        && b.ExpiryDate <= DateTime.UtcNow.AddDays(30)
        && b.CurrentQuantity > 0)
    .OrderBy(b => b.ExpiryDate)
    .ToListAsync();
```

---

## Step 8.5 — Stock Operations & Inventory Transactions

### Controller: `StockController`

| Method | Endpoint                  | Policy            | Description                     |
| ------ | ------------------------- | ----------------- | ------------------------------- |
| `GET`  | `/api/stock/levels`       | `ItemsView`       | Stock by item × location matrix |
| `POST` | `/api/stock/adjust`       | `ItemsEdit`       | Manual adjustment (±quantity)   |
| `POST` | `/api/stock/transfer`     | `TransfersCreate` | Transfer between locations      |
| `GET`  | `/api/stock/transactions` | `ItemsView`       | Transaction history (paginated) |

### Stock Adjustment Flow

```csharp
public async Task AdjustStock(AdjustStockRequest request)
{
    var itemLocation = await _db.ItemLocations
        .FirstOrDefaultAsync(il => il.ItemId == request.ItemId && il.LocationId == request.LocationId)
        ?? throw new NotFoundException("ItemLocation", $"{request.ItemId}/{request.LocationId}");

    itemLocation.Quantity += request.Quantity; // Can be negative for removal

    // Record transaction
    _db.InventoryTransactions.Add(new InventoryTransaction
    {
        ItemId = request.ItemId,
        LocationId = request.LocationId,
        BatchId = request.BatchId,
        TransactionType = request.Quantity > 0 ? TransactionType.Adjustment : TransactionType.Damage,
        Quantity = Math.Abs(request.Quantity),
        Notes = request.Notes,
        TransactionDate = DateTime.UtcNow
    });

    // Update batch quantity if batch-tracked
    if (request.BatchId.HasValue)
    {
        var batch = await _db.Batches.FindAsync(request.BatchId.Value);
        if (batch != null) batch.CurrentQuantity += request.Quantity;
    }

    await _db.SaveChangesAsync();
}
```

### Stock Transfer Flow

```csharp
public async Task TransferStock(TransferStockRequest request)
{
    // Deduct from source
    var source = await _db.ItemLocations
        .FirstAsync(il => il.ItemId == request.ItemId && il.LocationId == request.FromLocationId);
    if (source.Available < request.Quantity) throw new ConflictException("Insufficient available stock");
    source.Quantity -= request.Quantity;

    // Add to destination (create if doesn't exist)
    var dest = await _db.ItemLocations
        .FirstOrDefaultAsync(il => il.ItemId == request.ItemId && il.LocationId == request.ToLocationId);
    if (dest == null)
    {
        dest = new ItemLocation { ItemId = request.ItemId, LocationId = request.ToLocationId, Quantity = 0 };
        _db.ItemLocations.Add(dest);
    }
    dest.Quantity += request.Quantity;

    // Two transactions: TransferOut + TransferIn
    _db.InventoryTransactions.AddRange(
        new InventoryTransaction { ItemId = request.ItemId, LocationId = request.FromLocationId, TransactionType = TransactionType.TransferOut, Quantity = request.Quantity },
        new InventoryTransaction { ItemId = request.ItemId, LocationId = request.ToLocationId, TransactionType = TransactionType.TransferIn, Quantity = request.Quantity }
    );

    await _db.SaveChangesAsync();
}
```

---

## Step 8.6 — Barcode Scanning

### Controller: `BarcodeController`

| Method | Endpoint                     | Policy      | Description              |
| ------ | ---------------------------- | ----------- | ------------------------ |
| `GET`  | `/api/barcode/lookup/{code}` | `ItemsView` | Search by barcode or SKU |
| `POST` | `/api/barcode/quick-adjust`  | `ItemsEdit` | Scan → adjust stock flow |

Frontend uses device camera via [ZXing-js](https://github.com/nickyout/react-qr-reader) or [Html5-QRCode](https://github.com/nickyout/html5-qrcode).

---

## Verification Checklist

- [ ] CRUD operations for categories, items, locations, batches all work
- [ ] SKU uniqueness enforced per tenant
- [ ] Stock adjust/transfer updates `ItemLocation` and records `InventoryTransaction`
- [ ] Transfer validates available quantity before proceeding
- [ ] Low-stock and expiring-batch alert queries return correct results
- [ ] Barcode lookup returns item or 404
- [ ] Pagination, search, and filters work correctly on item list
- [ ] Image upload stores to Azure Blob and updates `ImageUrl`

---

→ [Phase 9 — Orders & Supply Chain](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/09-Orders-Supply-Chain.md)
