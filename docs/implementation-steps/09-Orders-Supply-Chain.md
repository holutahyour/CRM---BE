# Phase 9 — Orders & Supply Chain

> **Goal**: Implement purchase orders, sales orders, supplier management, receiving, fulfillment, and e-commerce integration.

---

## Step 9.1 — Supplier Management

### DTOs

```csharp
public record SupplierDto(int Id, string Name, string? SupplierCode, string? ContactPerson, string? Email, string? Phone, int ActiveOrderCount);
public record CreateSupplierRequest(string Name, string? SupplierCode, string? ContactPerson, string? Email, string? Phone, string? Address, string? TaxId, int? PaymentTermsDays);
```

### Controller: `SuppliersController`

| Method   | Endpoint              | Policy            | Description                  |
| -------- | --------------------- | ----------------- | ---------------------------- |
| `GET`    | `/api/suppliers`      | `SuppliersView`   | List (paginated, searchable) |
| `GET`    | `/api/suppliers/{id}` | `SuppliersView`   | Detail with order history    |
| `POST`   | `/api/suppliers`      | `SuppliersManage` | Create supplier              |
| `PUT`    | `/api/suppliers/{id}` | `SuppliersManage` | Update supplier              |
| `DELETE` | `/api/suppliers/{id}` | `SuppliersManage` | Soft-delete                  |

---

## Step 9.2 — Purchase Orders

### DTOs

```csharp
public record PurchaseOrderDto(int Id, string OrderNumber, string SupplierName, DateTime OrderDate, PurchaseOrderStatus Status, decimal TotalAmount, int ItemCount);
public record CreatePurchaseOrderRequest(int SupplierId, DateTime? ExpectedDeliveryDate, string? Notes, List<PurchaseOrderLineRequest> Lines);
public record PurchaseOrderLineRequest(int ItemId, decimal Quantity, decimal UnitPrice, int? LocationId);
public record ReceiveLineRequest(int PurchaseOrderItemId, decimal ReceivedQuantity, string? BatchNumber, DateTime? ExpiryDate, int? LocationId);
```

### Controller: `PurchaseOrdersController`

| Method | Endpoint                            | Policy                  | Description                 |
| ------ | ----------------------------------- | ----------------------- | --------------------------- |
| `GET`  | `/api/purchase-orders`              | `PurchaseOrdersView`    | List (filterable by status) |
| `GET`  | `/api/purchase-orders/{id}`         | `PurchaseOrdersView`    | Detail with line items      |
| `POST` | `/api/purchase-orders`              | `PurchaseOrdersCreate`  | Create order (Draft)        |
| `PUT`  | `/api/purchase-orders/{id}`         | `PurchaseOrdersCreate`  | Update draft order          |
| `POST` | `/api/purchase-orders/{id}/submit`  | `PurchaseOrdersCreate`  | Submit for approval         |
| `POST` | `/api/purchase-orders/{id}/approve` | `PurchaseOrdersApprove` | Approve order               |
| `POST` | `/api/purchase-orders/{id}/receive` | `PurchaseOrdersCreate`  | Receive items               |
| `POST` | `/api/purchase-orders/{id}/cancel`  | `PurchaseOrdersApprove` | Cancel order                |

### Auto-Generated Order Numbers

```csharp
private async Task<string> GenerateOrderNumber(string prefix)
{
    var year = DateTime.UtcNow.Year;
    var count = await _db.PurchaseOrders.CountAsync(o => o.OrderDate.Year == year) + 1;
    return $"{prefix}-{year}-{count:D5}";  // "PO-2026-00042"
}
```

### Receiving Flow

```csharp
public async Task ReceiveItems(int orderId, List<ReceiveLineRequest> lines)
{
    var order = await _db.PurchaseOrders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId)
        ?? throw new NotFoundException("PurchaseOrder", orderId);

    foreach (var line in lines)
    {
        var poItem = order.Items.First(i => i.Id == line.PurchaseOrderItemId);
        poItem.ReceivedQuantity += line.ReceivedQuantity;

        var locationId = line.LocationId ?? poItem.LocationId ?? throw new ConflictException("Location required");

        // Update stock
        var itemLoc = await _db.ItemLocations
            .FirstOrDefaultAsync(il => il.ItemId == poItem.ItemId && il.LocationId == locationId)
            ?? new ItemLocation { ItemId = poItem.ItemId, LocationId = locationId, Quantity = 0 };
        if (itemLoc.Id == 0) _db.ItemLocations.Add(itemLoc);
        itemLoc.Quantity += line.ReceivedQuantity;

        // Create batch if batch-tracked
        if (line.BatchNumber != null)
        {
            _db.Batches.Add(new Batch
            {
                ItemId = poItem.ItemId,
                BatchNumber = line.BatchNumber,
                ExpiryDate = line.ExpiryDate,
                ReceivedDate = DateTime.UtcNow,
                InitialQuantity = line.ReceivedQuantity,
                CurrentQuantity = line.ReceivedQuantity,
                SupplierBatchId = line.BatchNumber
            });
        }

        // Record transaction
        _db.InventoryTransactions.Add(new InventoryTransaction
        {
            ItemId = poItem.ItemId,
            LocationId = locationId,
            TransactionType = TransactionType.Purchase,
            Quantity = line.ReceivedQuantity,
            ReferenceId = orderId,
            TransactionDate = DateTime.UtcNow
        });
    }

    // Update order status
    var allReceived = order.Items.All(i => i.ReceivedQuantity >= i.Quantity);
    var anyReceived = order.Items.Any(i => i.ReceivedQuantity > 0);
    order.Status = allReceived ? PurchaseOrderStatus.Received
        : anyReceived ? PurchaseOrderStatus.PartiallyReceived
        : order.Status;
    order.ReceivedDate = allReceived ? DateTime.UtcNow : null;

    await _db.SaveChangesAsync();
}
```

---

## Step 9.3 — Sales Orders

### Controller: `SalesOrdersController`

| Method | Endpoint                         | Policy               | Description                  |
| ------ | -------------------------------- | -------------------- | ---------------------------- |
| `GET`  | `/api/sales-orders`              | `SalesOrdersView`    | List (filterable)            |
| `GET`  | `/api/sales-orders/{id}`         | `SalesOrdersView`    | Detail                       |
| `POST` | `/api/sales-orders`              | `SalesOrdersCreate`  | Create order                 |
| `POST` | `/api/sales-orders/{id}/fulfill` | `SalesOrdersFulfill` | Fulfill items (deduct stock) |
| `POST` | `/api/sales-orders/{id}/cancel`  | `SalesOrdersCreate`  | Cancel                       |

### Fulfillment Flow

Similar to receiving but reverses stock:

1. Deduct from `ItemLocation.Quantity`
2. Reserve stock (`ItemLocation.Reserved`) during processing
3. If batch-tracked, deduct from oldest batch first (FIFO)
4. Record `InventoryTransaction` with `TransactionType.Sale`

---

## Step 9.4 — E-Commerce Integration (Optional)

### Webhook Endpoint

```csharp
[HttpPost("/api/ecommerce/webhook")]
[AllowAnonymous]  // Secured via webhook secret validation
public async Task<IActionResult> IncomingOrder([FromBody] EcomWebhookPayload payload)
{
    // 1. Validate webhook signature (HMAC-SHA256)
    // 2. Map external order to SalesOrder
    // 3. Set EcomOrderId for deduplication
    // 4. Auto-confirm if stock available
}
```

### Supported Platforms (Future)

| Platform    | Integration Method | Key                    |
| ----------- | ------------------ | ---------------------- |
| Shopify     | Webhook + REST API | Orders, Inventory sync |
| WooCommerce | Webhook + REST API | Orders, Products       |
| Custom API  | Push/Pull          | Configurable mapping   |

---

## Verification Checklist

- [ ] Supplier CRUD works with proper permissions
- [ ] PO lifecycle: Draft → Submit → Approve → Receive → Completed
- [ ] Receiving creates `ItemLocation` records + `InventoryTransaction`
- [ ] Batch-tracked items generate `Batch` record on receive
- [ ] Order numbers auto-increment per year
- [ ] Sales order fulfillment deducts stock correctly
- [ ] Cancellation does not reverse already-received/fulfilled items
- [ ] E-commerce webhook validates signature and deduplicates by `EcomOrderId`

---

→ [Phase 10 — Reporting, Audit & Deployment](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/10-Reporting-Audit-Deployment.md)
