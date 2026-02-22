# Phase 3 — Infrastructure: Database & EF Core

> **Goal**: Implement `ImsDbContext`, entity type configurations, seed data, migrations, repositories, and interceptors in `IMS.Infrastructure`.

---

## Step 3.1 — Create `ImsDbContext`

**File**: `src/IMS.Infrastructure/Data/Context/ImsDbContext.cs`

```csharp
namespace IMS.Infrastructure.Data.Context;

using IMS.Domain.Entities.Common;
using IMS.Domain.Entities.Core;
using IMS.Domain.Entities.Inventory;
using IMS.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;

public class ImsDbContext(
    DbContextOptions<ImsDbContext> options,
    ITenantProvider tenantProvider
) : DbContext(options)
{
    // Core
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<MenuPermission> MenuPermissions => Set<MenuPermission>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<ModuleCategory> ModuleCategories => Set<ModuleCategory>();
    public DbSet<TenantModule> TenantModules => Set<TenantModule>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Inventory
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<ItemLocation> ItemLocations => Set<ItemLocation>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

    // Orders
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderItem> SalesOrderItems => Set<SalesOrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all IEntityTypeConfiguration classes from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ImsDbContext).Assembly);

        // Apply global tenant query filters
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ImsDbContext)
                    .GetMethod(nameof(ApplyTenantFilter),
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(this, [modelBuilder]);
            }
        }

        // Apply soft-delete filter for all BaseEntity types
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ImsDbContext)
                    .GetMethod(nameof(ApplySoftDeleteFilter),
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(this, [modelBuilder]);
            }
        }
    }

    private void ApplyTenantFilter<T>(ModelBuilder modelBuilder) where T : TenantEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e =>
            e.TenantId == tenantProvider.TenantId && !e.IsDeleted);
    }

    private void ApplySoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : BaseEntity
    {
        // Only apply if not already filtered by tenant (which includes IsDeleted)
        if (!typeof(TenantEntity).IsAssignableFrom(typeof(T)))
        {
            modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
```

**File**: `src/IMS.Application/Common/Interfaces/ITenantProvider.cs`

```csharp
namespace IMS.Application.Common.Interfaces;

public interface ITenantProvider
{
    int TenantId { get; }
    string? UserId { get; }  // Entra OID
}
```

## Step 3.2 — Entity Type Configurations

Create one configuration file per entity group. Below are the key configurations.

**File**: `src/IMS.Infrastructure/Data/Configurations/TenantConfiguration.cs`

```csharp
namespace IMS.Infrastructure.Data.Configurations;

using IMS.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Code).HasMaxLength(20);
        builder.Property(t => t.RegistrationNumber).HasMaxLength(50);
        builder.Property(t => t.IndustryType).HasMaxLength(50);
        builder.Property(t => t.Address).HasMaxLength(500);
        builder.Property(t => t.ContactEmail).HasMaxLength(150);
        builder.Property(t => t.ContactPhone).HasMaxLength(30);
        builder.Property(t => t.SubscriptionPlan).HasMaxLength(50);
        builder.Property(t => t.SubscriptionStatus)
            .HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.RowVersion).IsRowVersion();

        builder.HasIndex(t => t.Code).IsUnique()
            .HasFilter("[Code] IS NOT NULL");
    }
}
```

**File**: `src/IMS.Infrastructure/Data/Configurations/UserConfiguration.cs`

```csharp
namespace IMS.Infrastructure.Data.Configurations;

using IMS.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.EntraObjectId).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(150).IsRequired();
        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);
        builder.Property(u => u.Phone).HasMaxLength(30);
        builder.Property(u => u.AvatarUrl).HasMaxLength(500);
        builder.Property(u => u.RowVersion).IsRowVersion();

        builder.HasIndex(u => u.EntraObjectId).IsUnique();
        builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();
    }
}
```

**File**: `src/IMS.Infrastructure/Data/Configurations/ItemConfiguration.cs`

```csharp
namespace IMS.Infrastructure.Data.Configurations;

using IMS.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("items");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Sku).HasMaxLength(50).IsRequired();
        builder.Property(i => i.Name).HasMaxLength(200).IsRequired();
        builder.Property(i => i.UnitType).HasMaxLength(30).IsRequired();
        builder.Property(i => i.Barcode).HasMaxLength(100);
        builder.Property(i => i.ImageUrl).HasMaxLength(500);
        builder.Property(i => i.StorageConditions).HasMaxLength(500);
        builder.Property(i => i.MinStockLevel).HasPrecision(18, 4);
        builder.Property(i => i.ReorderQuantity).HasPrecision(18, 4);
        builder.Property(i => i.CostPrice).HasPrecision(18, 4);
        builder.Property(i => i.SellingPrice).HasPrecision(18, 4);
        builder.Property(i => i.RowVersion).IsRowVersion();

        builder.HasIndex(i => new { i.TenantId, i.Sku }).IsUnique();
        builder.HasIndex(i => i.Barcode);
        builder.HasIndex(i => i.Name);

        builder.HasOne(i => i.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Supplier)
            .WithMany()
            .HasForeignKey(i => i.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
```

> **Pattern**: Create similar configurations for `Role`, `Permission`, `Menu`, `Module`, `Location`, `Batch`, `ItemLocation`, `PurchaseOrder`, `SalesOrder`, `Supplier`, `AuditLog`, and all junction tables. Follow the same pattern of max lengths, precision, indexes, and relationships.

### Additional Configurations to Create

| File                                   | Entity                 | Key Configuration Notes                                         |
| -------------------------------------- | ---------------------- | --------------------------------------------------------------- |
| `RoleConfiguration.cs`                 | `Role`                 | Unique `(TenantId, Code)`, max lengths                          |
| `PermissionConfiguration.cs`           | `Permission`           | Unique `Code`, index on `ModuleCode`                            |
| `MenuConfiguration.cs`                 | `Menu`                 | Self-referencing FK `ParentId`, unique `Name`                   |
| `MenuPermissionConfiguration.cs`       | `MenuPermission`       | Composite unique `(MenuId, PermissionId)`                       |
| `UserRoleConfiguration.cs`             | `UserRole`             | Composite unique `(UserId, RoleId)`                             |
| `RolePermissionConfiguration.cs`       | `RolePermission`       | Composite unique `(RoleId, PermissionId)`                       |
| `ModuleConfiguration.cs`               | `Module`               | Unique `Code`, FK to `ModuleCategory`                           |
| `TenantModuleConfiguration.cs`         | `TenantModule`         | Composite unique `(TenantId, ModuleId)`                         |
| `LocationConfiguration.cs`             | `Location`             | Enum conversion for `Type`, precision for geo                   |
| `ItemLocationConfiguration.cs`         | `ItemLocation`         | Composite unique `(ItemId, LocationId)`, precision              |
| `BatchConfiguration.cs`                | `Batch`                | Composite unique `(ItemId, BatchNumber)`, index on `ExpiryDate` |
| `InventoryTransactionConfiguration.cs` | `InventoryTransaction` | BIGINT PK, index on `TransactionDate`, enum conversion          |
| `PurchaseOrderConfiguration.cs`        | `PurchaseOrder`        | Enum conversion for `Status`, FK to Supplier                    |
| `SalesOrderConfiguration.cs`           | `SalesOrder`           | Enum conversion, index on `EcomOrderId`                         |
| `AuditLogConfiguration.cs`             | `AuditLog`             | BIGINT PK, no soft-delete filter, index on `Timestamp`          |

---

## Step 3.3 — Seed Data

**File**: `src/IMS.Infrastructure/Data/Seed/ModuleSeedData.cs`

```csharp
namespace IMS.Infrastructure.Data.Seed;

using IMS.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;

public static class ModuleSeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // Module Categories
        modelBuilder.Entity<ModuleCategory>().HasData(
            new ModuleCategory { Id = 1, Code = "CORE", Name = "Core", IsActive = true },
            new ModuleCategory { Id = 2, Code = "OPS", Name = "Operations", IsActive = true },
            new ModuleCategory { Id = 3, Code = "SUPPLY", Name = "Supply Chain", IsActive = true },
            new ModuleCategory { Id = 4, Code = "SALES", Name = "Sales", IsActive = true },
            new ModuleCategory { Id = 5, Code = "INTEL", Name = "Intelligence", IsActive = true },
            new ModuleCategory { Id = 6, Code = "INTEG", Name = "Integrations", IsActive = true }
        );

        // Modules
        modelBuilder.Entity<Module>().HasData(
            new Module { Id = 1, Code = "CORE", Name = "Core & Administration", CategoryId = 1, Version = "1.0", IsActive = true },
            new Module { Id = 2, Code = "INVENTORY", Name = "Inventory Management", CategoryId = 2, Version = "1.0", IsActive = true },
            new Module { Id = 3, Code = "WAREHOUSE", Name = "Warehouse & Locations", CategoryId = 2, Version = "1.0", IsActive = true },
            new Module { Id = 4, Code = "PROCUREMENT", Name = "Procurement", CategoryId = 3, Version = "1.0", IsActive = true },
            new Module { Id = 5, Code = "SALES", Name = "Sales & Orders", CategoryId = 4, Version = "1.0", IsActive = true },
            new Module { Id = 6, Code = "SUPPLIERS", Name = "Supplier Management", CategoryId = 3, Version = "1.0", IsActive = true },
            new Module { Id = 7, Code = "REPORTS", Name = "Reporting & Analytics", CategoryId = 5, Version = "1.0", IsActive = true },
            new Module { Id = 8, Code = "ECOMMERCE", Name = "E-Commerce Integration", CategoryId = 6, Version = "1.0", IsActive = true },
            new Module { Id = 9, Code = "BARCODE", Name = "Barcode & Scanning", CategoryId = 2, Version = "1.0", IsActive = true }
        );
    }
}
```

**File**: `src/IMS.Infrastructure/Data/Seed/PermissionSeedData.cs`

```csharp
namespace IMS.Infrastructure.Data.Seed;

using IMS.Domain.Constants;
using IMS.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;

public static class PermissionSeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var id = 1;
        var permissions = new List<Permission>();

        // Generate from Permissions constants class using reflection
        foreach (var code in Permissions.All)
        {
            var parts = code.Split('.');
            var modulePart = parts[0].ToUpper() switch
            {
                "INVENTORY" => ModuleCodes.Inventory,
                "ORDERS" => parts[1] == "purchase" ? ModuleCodes.Procurement : ModuleCodes.Sales,
                "SUPPLIERS" => ModuleCodes.Suppliers,
                "REPORTS" => ModuleCodes.Reports,
                "ADMIN" => ModuleCodes.Core,
                _ => ModuleCodes.Core
            };

            permissions.Add(new Permission
            {
                Id = id++,
                Code = code,
                Name = FormatName(code),
                ModuleCode = modulePart,
            });
        }

        modelBuilder.Entity<Permission>().HasData(permissions.ToArray());
    }

    private static string FormatName(string code)
    {
        // "inventory.items.view" → "View Items"
        var parts = code.Split('.');
        var action = parts.Last();
        var resource = parts.Length > 1 ? parts[^2] : parts[0];
        return $"{char.ToUpper(action[0])}{action[1..]} {char.ToUpper(resource[0])}{resource[1..]}";
    }
}
```

**File**: `src/IMS.Infrastructure/Data/Seed/MenuSeedData.cs`

```csharp
namespace IMS.Infrastructure.Data.Seed;

using IMS.Domain.Constants;
using IMS.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;

public static class MenuSeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Menu>().HasData(
            // Root menus
            new Menu { Id = 1, Name = "DASHBOARD", Label = "Dashboard", Icon = "layout-dashboard", Route = "/dashboard", Position = 0, ModuleCode = ModuleCodes.Core },
            new Menu { Id = 2, Name = "INVENTORY", Label = "Inventory", Icon = "package", Position = 1, ModuleCode = ModuleCodes.Inventory },
            new Menu { Id = 3, Name = "LOCATIONS", Label = "Locations", Icon = "map-pin", Position = 2, ModuleCode = ModuleCodes.Warehouse },
            new Menu { Id = 4, Name = "SUPPLIERS", Label = "Suppliers", Icon = "truck", Position = 3, ModuleCode = ModuleCodes.Suppliers },
            new Menu { Id = 5, Name = "ORDERS", Label = "Orders", Icon = "clipboard-list", Position = 4 },
            new Menu { Id = 6, Name = "REPORTS", Label = "Reports", Icon = "bar-chart-3", Position = 5, ModuleCode = ModuleCodes.Reports },
            new Menu { Id = 7, Name = "ADMIN", Label = "Administration", Icon = "settings", Position = 6, ModuleCode = ModuleCodes.Core },

            // Inventory children
            new Menu { Id = 10, Name = "INVENTORY_ITEMS", Label = "Items", Icon = "box", Route = "/inventory/items", ParentId = 2, Position = 0 },
            new Menu { Id = 11, Name = "INVENTORY_CATEGORIES", Label = "Categories", Icon = "tags", Route = "/inventory/categories", ParentId = 2, Position = 1 },
            new Menu { Id = 12, Name = "INVENTORY_BATCHES", Label = "Batches & Lots", Icon = "layers", Route = "/inventory/batches", ParentId = 2, Position = 2 },
            new Menu { Id = 13, Name = "INVENTORY_STOCK", Label = "Stock Levels", Icon = "bar-chart", Route = "/inventory/stock", ParentId = 2, Position = 3 },
            new Menu { Id = 14, Name = "INVENTORY_SCANNER", Label = "Barcode Scanner", Icon = "scan", Route = "/inventory/scanner", ParentId = 2, Position = 4, ModuleCode = ModuleCodes.Barcode },

            // Locations children
            new Menu { Id = 20, Name = "LOCATIONS_ALL", Label = "All Locations", Icon = "building", Route = "/locations", ParentId = 3, Position = 0 },
            new Menu { Id = 21, Name = "LOCATIONS_TRANSFERS", Label = "Stock Transfers", Icon = "arrow-right-left", Route = "/locations/transfers", ParentId = 3, Position = 1 },

            // Suppliers children
            new Menu { Id = 30, Name = "SUPPLIERS_LIST", Label = "Supplier List", Icon = "users", Route = "/suppliers", ParentId = 4, Position = 0 },
            new Menu { Id = 31, Name = "SUPPLIERS_PERFORMANCE", Label = "Supplier Performance", Icon = "trending-up", Route = "/suppliers/performance", ParentId = 4, Position = 1 },

            // Orders children
            new Menu { Id = 40, Name = "ORDERS_PURCHASE", Label = "Purchase Orders", Icon = "shopping-cart", Route = "/orders/purchase", ParentId = 5, Position = 0, ModuleCode = ModuleCodes.Procurement },
            new Menu { Id = 41, Name = "ORDERS_SALES", Label = "Sales Orders", Icon = "receipt", Route = "/orders/sales", ParentId = 5, Position = 1, ModuleCode = ModuleCodes.Sales },
            new Menu { Id = 42, Name = "ORDERS_ECOM", Label = "E-Commerce Sync", Icon = "globe", Route = "/orders/ecommerce", ParentId = 5, Position = 2, ModuleCode = ModuleCodes.ECommerce },

            // Reports children
            new Menu { Id = 50, Name = "REPORTS_STOCK", Label = "Stock Report", Icon = "file-bar-chart", Route = "/reports/stock", ParentId = 6, Position = 0 },
            new Menu { Id = 51, Name = "REPORTS_EXPIRATION", Label = "Expiration Alerts", Icon = "alert-triangle", Route = "/reports/expiration", ParentId = 6, Position = 1 },
            new Menu { Id = 52, Name = "REPORTS_MOVEMENTS", Label = "Movement History", Icon = "history", Route = "/reports/movements", ParentId = 6, Position = 2 },
            new Menu { Id = 53, Name = "REPORTS_SALES", Label = "Sales Trends", Icon = "trending-up", Route = "/reports/sales-trends", ParentId = 6, Position = 3 },
            new Menu { Id = 54, Name = "REPORTS_TRACEABILITY", Label = "Traceability", Icon = "search", Route = "/reports/traceability", ParentId = 6, Position = 4 },

            // Admin children
            new Menu { Id = 60, Name = "ADMIN_USERS", Label = "Users", Icon = "users", Route = "/admin/users", ParentId = 7, Position = 0 },
            new Menu { Id = 61, Name = "ADMIN_ROLES", Label = "Roles & Permissions", Icon = "shield", Route = "/admin/roles", ParentId = 7, Position = 1 },
            new Menu { Id = 62, Name = "ADMIN_MODULES", Label = "Modules", Icon = "puzzle", Route = "/admin/modules", ParentId = 7, Position = 2 },
            new Menu { Id = 63, Name = "ADMIN_SETTINGS", Label = "Tenant Settings", Icon = "sliders", Route = "/admin/settings", ParentId = 7, Position = 3 },
            new Menu { Id = 64, Name = "ADMIN_AUDIT", Label = "Audit Logs", Icon = "scroll", Route = "/admin/audit", ParentId = 7, Position = 4 }
        );
    }
}
```

Call all seed methods from `ImsDbContext.OnModelCreating()`:

```csharp
// ... at the end of OnModelCreating
ModuleSeedData.Seed(modelBuilder);
PermissionSeedData.Seed(modelBuilder);
MenuSeedData.Seed(modelBuilder);

// Optional: Seed the first system tenant and its admin role permissions
// RolePermissionSeedData.Seed(modelBuilder);
```

---

## Step 3.4 — Audit Interceptor

**File**: `src/IMS.Infrastructure/Data/Interceptors/AuditSaveChangesInterceptor.cs`

```csharp
namespace IMS.Infrastructure.Data.Interceptors;

using System.Text.Json;
using IMS.Domain.Entities.Common;
using IMS.Domain.Entities.Core;
using IMS.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class AuditSaveChangesInterceptor(ITenantProvider tenantProvider) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        var context = eventData.Context;
        if (context == null) return await base.SavingChangesAsync(eventData, result, ct);

        // Set audit fields
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = tenantProvider.UserId;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = tenantProvider.UserId;
                    break;
            }
        }

        // Set tenant_id on new tenant entities
        foreach (var entry in context.ChangeTracker.Entries<TenantEntity>()
            .Where(e => e.State == EntityState.Added && e.Entity.TenantId == 0))
        {
            entry.Entity.TenantId = tenantProvider.TenantId;
        }

        // Build audit logs
        var auditEntries = new List<AuditLog>();
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            if (entry.Entity is AuditLog) continue; // Don't audit audit logs

            var audit = new AuditLog
            {
                TenantId = tenantProvider.TenantId,
                UserId = tenantProvider.UserId,
                EntityName = entry.Entity.GetType().Name,
                EntityId = entry.Property("Id").CurrentValue?.ToString(),
                Action = entry.State.ToString(),
                Timestamp = DateTime.UtcNow
            };

            if (entry.State == EntityState.Modified)
            {
                var changed = entry.Properties.Where(p => p.IsModified).ToList();
                audit.OldValues = JsonSerializer.Serialize(
                    changed.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue?.ToString()));
                audit.NewValues = JsonSerializer.Serialize(
                    changed.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue?.ToString()));
                audit.ChangedProperties = JsonSerializer.Serialize(
                    changed.Select(p => p.Metadata.Name));
            }
            else if (entry.State == EntityState.Added)
            {
                audit.NewValues = JsonSerializer.Serialize(
                    entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue?.ToString()));
            }

            auditEntries.Add(audit);
        }

        context.Set<AuditLog>().AddRange(auditEntries);

        return await base.SavingChangesAsync(eventData, result, ct);
    }
}
```

---

## Step 3.5 — Repository Interfaces & Implementations

**File**: `src/IMS.Application/Common/Interfaces/IRepository.cs`

```csharp
namespace IMS.Application.Common.Interfaces;

using System.Linq.Expressions;
using IMS.Domain.Entities.Common;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);  // Soft delete
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);
    IQueryable<T> Query();
}
```

**File**: `src/IMS.Application/Common/Interfaces/IUnitOfWork.cs`

```csharp
namespace IMS.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

**File**: `src/IMS.Infrastructure/Repositories/GenericRepository.cs`

```csharp
namespace IMS.Infrastructure.Repositories;

using System.Linq.Expressions;
using IMS.Application.Common.Interfaces;
using IMS.Domain.Entities.Common;
using IMS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

public class GenericRepository<T>(ImsDbContext context) : IRepository<T> where T : BaseEntity
{
    protected readonly ImsDbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _dbSet.FindAsync([id], ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await _dbSet.ToListAsync(ct);

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.Where(predicate).ToListAsync(ct);

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
        return entity;
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        entity.IsDeleted = true;
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.AnyAsync(predicate, ct);

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
        => predicate == null ? await _dbSet.CountAsync(ct) : await _dbSet.CountAsync(predicate, ct);

    public IQueryable<T> Query() => _dbSet.AsQueryable();
}
```

---

## Step 3.6 — DI Registration Extension

**File**: `src/IMS.Infrastructure/Extensions/InfrastructureServiceExtensions.cs`

```csharp
namespace IMS.Infrastructure.Extensions;

using IMS.Application.Common.Interfaces;
using IMS.Infrastructure.Data.Context;
using IMS.Infrastructure.Data.Interceptors;
using IMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // EF Core
        services.AddScoped<AuditSaveChangesInterceptor>();
        services.AddDbContext<ImsDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditSaveChangesInterceptor>();
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.EnableRetryOnFailure(3))
                .AddInterceptors(interceptor);
        });

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ImsDbContext>());

        return services;
    }
}
```

---

## Step 3.7 — Create Initial Migration

```bash
dotnet ef migrations add InitialCreate \
    --project src/IMS.Infrastructure \
    --startup-project src/IMS.API \
    --output-dir Data/Migrations

dotnet ef database update \
    --project src/IMS.Infrastructure \
    --startup-project src/IMS.API
```

---

## Verification Checklist

- [ ] `dotnet build src/IMS.Infrastructure` succeeds
- [ ] Migration generates successfully with all tables
- [ ] Database creates with correct schema + seed data
- [ ] Seed data includes modules, permissions, and menus
- [ ] Global query filters work (tenant + soft-delete)
- [ ] Audit interceptor captures changes on `SaveChangesAsync()`
- [ ] Repository CRUD operations work against test data

---

## Next Phase

→ [Phase 4 — Authentication: Azure Entra ID](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/04-Authentication-Entra-ID.md)
