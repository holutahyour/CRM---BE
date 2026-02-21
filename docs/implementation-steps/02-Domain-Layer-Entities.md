# Phase 2 — Domain Layer: Entities & Enums

> **Goal**: Implement all domain entities, enums, and constants in `IMS.Domain`. This project has zero external dependencies — only pure C# classes.

---

## Step 2.1 — Core Enums

**File**: `src/IMS.Domain/Enums/TransactionType.cs`

```csharp
namespace IMS.Domain.Enums;

public enum TransactionType
{
    Purchase = 1,
    Sale = 2,
    TransferIn = 3,
    TransferOut = 4,
    Adjustment = 5,
    Damage = 6,
    Return = 7,
    Production = 8,
    Consumption = 9
}
```

**File**: `src/IMS.Domain/Enums/OrderStatus.cs`

```csharp
namespace IMS.Domain.Enums;

public enum PurchaseOrderStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Ordered = 4,
    PartiallyReceived = 5,
    Received = 6,
    Cancelled = 7
}

public enum SalesOrderStatus
{
    Draft = 1,
    Confirmed = 2,
    Processing = 3,
    PartiallyFulfilled = 4,
    Fulfilled = 5,
    Shipped = 6,
    Delivered = 7,
    Cancelled = 8
}
```

**File**: `src/IMS.Domain/Enums/LocationType.cs`

```csharp
namespace IMS.Domain.Enums;

public enum LocationType
{
    Warehouse = 1,
    ColdStorage = 2,
    OpenField = 3,
    Silo = 4,
    PackingArea = 5,
    ProcessingPlant = 6,
    RetailStore = 7
}
```

**File**: `src/IMS.Domain/Enums/AuditAction.cs`

```csharp
namespace IMS.Domain.Enums;

public enum AuditAction
{
    Create = 1,
    Update = 2,
    Delete = 3
}
```

**File**: `src/IMS.Domain/Enums/SubscriptionStatus.cs`

```csharp
namespace IMS.Domain.Enums;

public enum SubscriptionStatus
{
    Active = 1,
    Trial = 2,
    Expired = 3,
    Suspended = 4
}
```

---

## Step 2.2 — Permission & Module Constants

**File**: `src/IMS.Domain/Constants/Permissions.cs`

```csharp
namespace IMS.Domain.Constants;

/// <summary>
/// All fine-grained permission codes used throughout the system.
/// These are seeded into the database and checked by authorization handlers.
/// </summary>
public static class Permissions
{
    // Inventory
    public const string ItemsView = "inventory.items.view";
    public const string ItemsCreate = "inventory.items.create";
    public const string ItemsEdit = "inventory.items.edit";
    public const string ItemsDelete = "inventory.items.delete";

    // Categories
    public const string CategoriesView = "inventory.categories.view";
    public const string CategoriesManage = "inventory.categories.manage";

    // Batches
    public const string BatchesView = "inventory.batches.view";
    public const string BatchesManage = "inventory.batches.manage";

    // Locations
    public const string LocationsView = "inventory.locations.view";
    public const string LocationsManage = "inventory.locations.manage";
    public const string TransfersCreate = "inventory.transfers.create";

    // Suppliers
    public const string SuppliersView = "suppliers.view";
    public const string SuppliersManage = "suppliers.manage";

    // Purchase Orders
    public const string PurchaseOrdersView = "orders.purchase.view";
    public const string PurchaseOrdersCreate = "orders.purchase.create";
    public const string PurchaseOrdersApprove = "orders.purchase.approve";

    // Sales Orders
    public const string SalesOrdersView = "orders.sales.view";
    public const string SalesOrdersCreate = "orders.sales.create";
    public const string SalesOrdersFulfill = "orders.sales.fulfill";

    // Reports
    public const string ReportsView = "reports.view";
    public const string ReportsExport = "reports.export";

    // Administration
    public const string UsersManage = "admin.users.manage";
    public const string RolesManage = "admin.roles.manage";
    public const string SettingsManage = "admin.settings.manage";
    public const string AuditView = "admin.audit.view";
    public const string ModulesManage = "admin.modules.manage";

    /// <summary>Returns all permission codes as a flat list (useful for seeding).</summary>
    public static IReadOnlyList<string> All => typeof(Permissions)
        .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
        .Where(f => f.FieldType == typeof(string))
        .Select(f => (string)f.GetValue(null)!)
        .ToList();
}
```

**File**: `src/IMS.Domain/Constants/ModuleCodes.cs`

```csharp
namespace IMS.Domain.Constants;

public static class ModuleCodes
{
    public const string Core = "CORE";
    public const string Inventory = "INVENTORY";
    public const string Warehouse = "WAREHOUSE";
    public const string Procurement = "PROCUREMENT";
    public const string Sales = "SALES";
    public const string Suppliers = "SUPPLIERS";
    public const string Reports = "REPORTS";
    public const string ECommerce = "ECOMMERCE";
    public const string Barcode = "BARCODE";
}
```

---

## Step 2.3 — Core Entities (Tenant, User, Role, Permission, Menu, Module)

**File**: `src/IMS.Domain/Entities/Core/Tenant.cs`

```csharp
namespace IMS.Domain.Entities.Core;

using IMS.Domain.Entities.Common;
using IMS.Domain.Enums;

public class Tenant : BaseEntity  // NOT TenantEntity — this IS the tenant
{
    public string Name { get; set; } = "";
    public string? Code { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? IndustryType { get; set; }
    public string? Address { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? SubscriptionPlan { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Trial;
    public DateTime? TrialEndDate { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; } = [];
    public ICollection<TenantModule> TenantModules { get; set; } = [];
}
```

**File**: `src/IMS.Domain/Entities/Core/User.cs`

```csharp
namespace IMS.Domain.Entities.Core;

using IMS.Domain.Entities.Common;

public class User : TenantEntity
{
    public string EntraObjectId { get; set; } = "";
    public string Email { get; set; } = "";
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // Computed
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
```

**File**: `src/IMS.Domain/Entities/Core/Role.cs`

```csharp
namespace IMS.Domain.Entities.Core;

using IMS.Domain.Entities.Common;

public class Role : TenantEntity
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
```

**File**: `src/IMS.Domain/Entities/Core/Permission.cs`

```csharp
namespace IMS.Domain.Entities.Core;

using IMS.Domain.Entities.Common;

public class Permission : BaseEntity  // Global — not tenant-scoped
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string? ModuleCode { get; set; }
    public string? Description { get; set; }

    // Navigation
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public ICollection<MenuPermission> MenuPermissions { get; set; } = [];
}
```

**File**: `src/IMS.Domain/Entities/Core/UserRole.cs`

```csharp
namespace IMS.Domain.Entities.Core;

using IMS.Domain.Entities.Common;

public class UserRole : TenantEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
```

**File**: `src/IMS.Domain/Entities/Core/RolePermission.cs`

```csharp
namespace IMS.Domain.Entities.Core;

using IMS.Domain.Entities.Common;

public class RolePermission : TenantEntity
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
```

**File**: `src/IMS.Domain/Entities/Core/Menu.cs`

```csharp
namespace IMS.Domain.Entities.Core;

using IMS.Domain.Entities.Common;

public class Menu : BaseEntity  // Global — not tenant-scoped
{
    public string Name { get; set; } = "";
    public string Label { get; set; } = "";
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public int? ParentId { get; set; }
    public string? ModuleCode { get; set; }
    public int Position { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Menu? Parent { get; set; }
    public ICollection<Menu> Children { get; set; } = [];
    public ICollection<MenuPermission> MenuPermissions { get; set; } = [];
}
```

**File**: `src/IMS.Domain/Entities/Core/MenuPermission.cs`

```csharp
namespace IMS.Domain.Entities.Core;

using IMS.Domain.Entities.Common;

public class MenuPermission : BaseEntity  // Global
{
    public int MenuId { get; set; }
    public int PermissionId { get; set; }

    public Menu Menu { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
```

**File**: `src/IMS.Domain/Entities/Core/Module.cs`

```csharp
namespace IMS.Domain.Entities.Core;

using IMS.Domain.Entities.Common;

public class Module : BaseEntity  // Global
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string? Description { get; set; }
    public string? Version { get; set; }
    public int? CategoryId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ModuleCategory? Category { get; set; }
    public ICollection<TenantModule> TenantModules { get; set; } = [];
}
```

**File**: `src/IMS.Domain/Entities/Core/ModuleCategory.cs`

```csharp
namespace IMS.Domain.Entities.Core;

using IMS.Domain.Entities.Common;

public class ModuleCategory : BaseEntity
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Module> Modules { get; set; } = [];
}
```

**File**: `src/IMS.Domain/Entities/Core/TenantModule.cs`

```csharp
namespace IMS.Domain.Entities.Core;

using IMS.Domain.Entities.Common;

public class TenantModule : TenantEntity
{
    public int ModuleId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ActivatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public Module Module { get; set; } = null!;
}
```

---

## Step 2.4 — Inventory Entities

**File**: `src/IMS.Domain/Entities/Inventory/Category.cs`

```csharp
namespace IMS.Domain.Entities.Inventory;

using IMS.Domain.Entities.Common;

public class Category : TenantEntity
{
    public string Name { get; set; } = "";
    public int? ParentId { get; set; }
    public string? Description { get; set; }

    // Navigation
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = [];
    public ICollection<Item> Items { get; set; } = [];
}
```

**File**: `src/IMS.Domain/Entities/Inventory/Item.cs`

```csharp
namespace IMS.Domain.Entities.Inventory;

using IMS.Domain.Entities.Common;
using IMS.Domain.Entities.Orders;

public class Item : TenantEntity
{
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public int? CategoryId { get; set; }
    public int? SupplierId { get; set; }
    public string? Description { get; set; }
    public string UnitType { get; set; } = "piece";
    public string? Barcode { get; set; }
    public bool BatchTracked { get; set; }
    public bool ExpiryTracked { get; set; }
    public decimal? MinStockLevel { get; set; }
    public decimal? ReorderQuantity { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public string? ImageUrl { get; set; }
    public string? StorageConditions { get; set; }

    // Navigation
    public Category? Category { get; set; }
    public Supplier? Supplier { get; set; }
    public ICollection<ItemLocation> ItemLocations { get; set; } = [];
    public ICollection<Batch> Batches { get; set; } = [];
    public ICollection<InventoryTransaction> Transactions { get; set; } = [];
}
```

**File**: `src/IMS.Domain/Entities/Inventory/Location.cs`

```csharp
namespace IMS.Domain.Entities.Inventory;

using IMS.Domain.Entities.Common;
using IMS.Domain.Enums;

public class Location : TenantEntity
{
    public string Name { get; set; } = "";
    public string? Code { get; set; }
    public LocationType Type { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Capacity { get; set; }
    public bool TemperatureControl { get; set; }

    // Navigation
    public ICollection<ItemLocation> ItemLocations { get; set; } = [];
}
```

**File**: `src/IMS.Domain/Entities/Inventory/ItemLocation.cs`

```csharp
namespace IMS.Domain.Entities.Inventory;

using IMS.Domain.Entities.Common;

public class ItemLocation : TenantEntity
{
    public int ItemId { get; set; }
    public int LocationId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Reserved { get; set; }
    public decimal Damaged { get; set; }

    // Computed
    public decimal Available => Quantity - Reserved - Damaged;

    // Navigation
    public Item Item { get; set; } = null!;
    public Location Location { get; set; } = null!;
}
```

**File**: `src/IMS.Domain/Entities/Inventory/Batch.cs`

```csharp
namespace IMS.Domain.Entities.Inventory;

using IMS.Domain.Entities.Common;

public class Batch : TenantEntity
{
    public int ItemId { get; set; }
    public string BatchNumber { get; set; } = "";
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public decimal? InitialQuantity { get; set; }
    public decimal CurrentQuantity { get; set; }
    public string? SupplierBatchId { get; set; }

    // Navigation
    public Item Item { get; set; } = null!;
}
```

**File**: `src/IMS.Domain/Entities/Inventory/InventoryTransaction.cs`

```csharp
namespace IMS.Domain.Entities.Inventory;

using IMS.Domain.Entities.Common;
using IMS.Domain.Enums;

public class InventoryTransaction : TenantEntity
{
    public new long Id { get; set; }  // BIGINT for high volume
    public int ItemId { get; set; }
    public int? BatchId { get; set; }
    public int? LocationId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Quantity { get; set; }
    public int? ReferenceId { get; set; }
    public string? Notes { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    // Navigation
    public Item Item { get; set; } = null!;
    public Batch? Batch { get; set; }
    public Location? Location { get; set; }
}
```

---

## Step 2.5 — Order Entities

**File**: `src/IMS.Domain/Entities/Orders/Supplier.cs`

```csharp
namespace IMS.Domain.Entities.Orders;

using IMS.Domain.Entities.Common;

public class Supplier : TenantEntity
{
    public string Name { get; set; } = "";
    public string? SupplierCode { get; set; }
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? TaxId { get; set; }
    public int? PaymentTermsDays { get; set; }

    // Navigation
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = [];
}
```

**File**: `src/IMS.Domain/Entities/Orders/PurchaseOrder.cs`

```csharp
namespace IMS.Domain.Entities.Orders;

using IMS.Domain.Entities.Common;
using IMS.Domain.Enums;

public class PurchaseOrder : TenantEntity
{
    public string OrderNumber { get; set; } = "";
    public int SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string? QualityNotes { get; set; }

    // Navigation
    public Supplier Supplier { get; set; } = null!;
    public ICollection<PurchaseOrderItem> Items { get; set; } = [];
}
```

**File**: `src/IMS.Domain/Entities/Orders/PurchaseOrderItem.cs`

```csharp
namespace IMS.Domain.Entities.Orders;

using IMS.Domain.Entities.Common;
using IMS.Domain.Entities.Inventory;

public class PurchaseOrderItem : TenantEntity
{
    public int PurchaseOrderId { get; set; }
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public int? LocationId { get; set; }
    public string? BatchNumber { get; set; }

    // Computed
    public decimal LineTotal => Quantity * UnitPrice;

    // Navigation
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Item Item { get; set; } = null!;
    public Location? Location { get; set; }
}
```

**File**: `src/IMS.Domain/Entities/Orders/SalesOrder.cs`

```csharp
namespace IMS.Domain.Entities.Orders;

using IMS.Domain.Entities.Common;
using IMS.Domain.Enums;

public class SalesOrder : TenantEntity
{
    public string OrderNumber { get; set; } = "";
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public DateTime OrderDate { get; set; }
    public SalesOrderStatus Status { get; set; } = SalesOrderStatus.Draft;
    public decimal TotalAmount { get; set; }
    public string? EcomOrderId { get; set; }
    public int? DeliveryLocationId { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public ICollection<SalesOrderItem> Items { get; set; } = [];
}
```

**File**: `src/IMS.Domain/Entities/Orders/SalesOrderItem.cs`

```csharp
namespace IMS.Domain.Entities.Orders;

using IMS.Domain.Entities.Common;
using IMS.Domain.Entities.Inventory;

public class SalesOrderItem : TenantEntity
{
    public int SalesOrderId { get; set; }
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal FulfilledQuantity { get; set; }
    public int? BatchId { get; set; }

    // Computed
    public decimal LineTotal => Quantity * UnitPrice;

    // Navigation
    public SalesOrder SalesOrder { get; set; } = null!;
    public Item Item { get; set; } = null!;
    public Batch? Batch { get; set; }
}
```

---

## Step 2.6 — Audit Entity

**File**: `src/IMS.Domain/Entities/Core/AuditLog.cs`

```csharp
namespace IMS.Domain.Entities.Core;

public class AuditLog
{
    public long Id { get; set; }
    public int TenantId { get; set; }
    public string? UserId { get; set; }
    public string? UserEmail { get; set; }
    public string EntityName { get; set; } = "";
    public string? EntityId { get; set; }
    public string Action { get; set; } = "";
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? ChangedProperties { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
```

---

## Step 2.7 — Domain Exceptions

**File**: `src/IMS.Domain/Exceptions/NotFoundException.cs`

```csharp
namespace IMS.Domain.Exceptions;

public class NotFoundException(string entityName, object key)
    : Exception($"Entity \"{entityName}\" with key ({key}) was not found.")
{
    public string EntityName { get; } = entityName;
    public object Key { get; } = key;
}
```

**File**: `src/IMS.Domain/Exceptions/ConflictException.cs`

```csharp
namespace IMS.Domain.Exceptions;

public class ConflictException(string message) : Exception(message);
```

**File**: `src/IMS.Domain/Exceptions/ForbiddenException.cs`

```csharp
namespace IMS.Domain.Exceptions;

public class ForbiddenException(string message = "You do not have permission to perform this action.")
    : Exception(message);
```

---

## Verification Checklist

- [ ] `dotnet build src/IMS.Domain` succeeds with zero errors
- [ ] No external NuGet packages referenced in `IMS.Domain.csproj`
- [ ] All entities inherit from `BaseEntity` or `TenantEntity`
- [ ] All navigation properties use collection initializers (`= []`)
- [ ] `Permissions.All` returns a complete list of permission codes
- [ ] Entity hierarchy: `BaseEntity` ← `TenantEntity` ← concrete entities

---

## Next Phase

→ [Phase 3 — Infrastructure: Database & EF Core](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/03-Infrastructure-Database.md)
