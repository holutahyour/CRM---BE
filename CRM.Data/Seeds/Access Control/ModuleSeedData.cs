using CRM.Domain.Constants;

namespace CRM.Data.Seeds.Access_Control;

public static class ModuleSeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // Module Categories
        modelBuilder.Entity<ModuleCategory>().HasData(
            new ModuleCategory { Id = Guid.NewGuid(), Code = "CORE", Name = "Core", IsActive = true },
            new ModuleCategory { Id = Guid.NewGuid(), Code = "OPS", Name = "Operations", IsActive = true },
            new ModuleCategory { Id = Guid.NewGuid(), Code = "SUPPLY", Name = "Supply Chain", IsActive = true },
            new ModuleCategory { Id = Guid.NewGuid(), Code = "SALES", Name = "Sales", IsActive = true },
            new ModuleCategory { Id = Guid.NewGuid(), Code = "INTEL", Name = "Intelligence", IsActive = true },
            new ModuleCategory { Id = Guid.NewGuid(), Code = "INTEG", Name = "Integrations", IsActive = true }
        );

        // Modules
        modelBuilder.Entity<Module>().HasData(
            new Module { Id = Guid.NewGuid(), Code = "CORE", Name = "Core & Administration", CategoryId = Guid.NewGuid(), Version = "1.0", IsActive = true },
            new Module { Id = Guid.NewGuid(), Code = "INVENTORY", Name = "Inventory Management", CategoryId = Guid.NewGuid(), Version = "1.0", IsActive = true },
            new Module { Id = Guid.NewGuid(), Code = "WAREHOUSE", Name = "Warehouse & Locations", CategoryId = Guid.NewGuid(), Version = "1.0", IsActive = true },
            new Module { Id = Guid.NewGuid(), Code = "PROCUREMENT", Name = "Procurement", CategoryId = Guid.NewGuid(), Version = "1.0", IsActive = true },
            new Module { Id = Guid.NewGuid(), Code = "SALES", Name = "Sales & Orders", CategoryId = Guid.NewGuid(), Version = "1.0", IsActive = true },
            new Module { Id = Guid.NewGuid(), Code = "SUPPLIERS", Name = "Supplier Management", CategoryId = Guid.NewGuid(), Version = "1.0", IsActive = true },
            new Module { Id = Guid.NewGuid(), Code = "REPORTS", Name = "Reporting & Analytics", CategoryId = Guid.NewGuid(), Version = "1.0", IsActive = true },
            new Module { Id = Guid.NewGuid(), Code = "ECOMMERCE", Name = "E-Commerce Integration", CategoryId = Guid.NewGuid(), Version = "1.0", IsActive = true },
            new Module { Id = Guid.NewGuid(), Code = "BARCODE", Name = "Barcode & Scanning", CategoryId = Guid.NewGuid(), Version = "1.0", IsActive = true }
        );
    }
}

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
                Id = Guid.NewGuid(),
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
public static class MenuSeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Menu>().HasData(
            // Root menus
            new Menu { Id = Guid.NewGuid(), Name = "DASHBOARD", Label = "Dashboard", Icon = "layout-dashboard", Route = "/dashboard", Position = 0, ModuleCode = ModuleCodes.Core },
            new Menu { Id = Guid.NewGuid(), Name = "INVENTORY", Label = "Inventory", Icon = "package", Position = 1, ModuleCode = ModuleCodes.Inventory },
            new Menu { Id = Guid.NewGuid(), Name = "LOCATIONS", Label = "Locations", Icon = "map-pin", Position = 2, ModuleCode = ModuleCodes.Warehouse },
            new Menu { Id = Guid.NewGuid(), Name = "SUPPLIERS", Label = "Suppliers", Icon = "truck", Position = 3, ModuleCode = ModuleCodes.Suppliers },
            new Menu { Id = Guid.NewGuid(), Name = "ORDERS", Label = "Orders", Icon = "clipboard-list", Position = 4 },
            new Menu { Id = Guid.NewGuid(), Name = "REPORTS", Label = "Reports", Icon = "bar-chart-3", Position = 5, ModuleCode = ModuleCodes.Reports },
            new Menu { Id = Guid.NewGuid(), Name = "ADMIN", Label = "Administration", Icon = "settings", Position = 6, ModuleCode = ModuleCodes.Core },

            // Inventory children
            new Menu { Id = Guid.NewGuid(), Name = "INVENTORY_ITEMS", Label = "Items", Icon = "box", Route = "/inventory/items", ParentId = Guid.NewGuid(), Position = 0 },
            new Menu { Id = Guid.NewGuid(), Name = "INVENTORY_CATEGORIES", Label = "Categories", Icon = "tags", Route = "/inventory/categories", ParentId = Guid.NewGuid(), Position = 1 },
            new Menu { Id = Guid.NewGuid(), Name = "INVENTORY_BATCHES", Label = "Batches & Lots", Icon = "layers", Route = "/inventory/batches", ParentId = Guid.NewGuid(), Position = 2 },
            new Menu { Id = Guid.NewGuid(), Name = "INVENTORY_STOCK", Label = "Stock Levels", Icon = "bar-chart", Route = "/inventory/stock", ParentId = Guid.NewGuid(), Position = 3 },
            new Menu { Id = Guid.NewGuid(), Name = "INVENTORY_SCANNER", Label = "Barcode Scanner", Icon = "scan", Route = "/inventory/scanner", ParentId = Guid.NewGuid(), Position = 4, ModuleCode = ModuleCodes.Barcode },

            // Locations children
            new Menu { Id = Guid.NewGuid(), Name = "LOCATIONS_ALL", Label = "All Locations", Icon = "building", Route = "/locations", ParentId = Guid.NewGuid(), Position = 0 },
            new Menu { Id = Guid.NewGuid(), Name = "LOCATIONS_TRANSFERS", Label = "Stock Transfers", Icon = "arrow-right-left", Route = "/locations/transfers", ParentId = Guid.NewGuid(), Position = 1 },

            // Suppliers children
            new Menu { Id = Guid.NewGuid(), Name = "SUPPLIERS_LIST", Label = "Supplier List", Icon = "users", Route = "/suppliers", ParentId = Guid.NewGuid(), Position = 0 },
            new Menu { Id = Guid.NewGuid(), Name = "SUPPLIERS_PERFORMANCE", Label = "Supplier Performance", Icon = "trending-up", Route = "/suppliers/performance", ParentId = Guid.NewGuid(), Position = 1 },

            // Orders children
            new Menu { Id = Guid.NewGuid(), Name = "ORDERS_PURCHASE", Label = "Purchase Orders", Icon = "shopping-cart", Route = "/orders/purchase", ParentId = Guid.NewGuid(), Position = 0, ModuleCode = ModuleCodes.Procurement },
            new Menu { Id = Guid.NewGuid(), Name = "ORDERS_SALES", Label = "Sales Orders", Icon = "receipt", Route = "/orders/sales", ParentId = Guid.NewGuid(), Position = 1, ModuleCode = ModuleCodes.Sales },
            new Menu { Id = Guid.NewGuid(), Name = "ORDERS_ECOM", Label = "E-Commerce Sync", Icon = "globe", Route = "/orders/ecommerce", ParentId = Guid.NewGuid(), Position = 2, ModuleCode = ModuleCodes.ECommerce },

            // Reports children
            new Menu { Id = Guid.NewGuid(), Name = "REPORTS_STOCK", Label = "Stock Report", Icon = "file-bar-chart", Route = "/reports/stock", ParentId = Guid.NewGuid(), Position = 0 },
            new Menu { Id = Guid.NewGuid(), Name = "REPORTS_EXPIRATION", Label = "Expiration Alerts", Icon = "alert-triangle", Route = "/reports/expiration", ParentId = Guid.NewGuid(), Position = 1 },
            new Menu { Id = Guid.NewGuid(), Name = "REPORTS_MOVEMENTS", Label = "Movement History", Icon = "history", Route = "/reports/movements", ParentId = Guid.NewGuid(), Position = 2 },
            new Menu { Id = Guid.NewGuid(), Name = "REPORTS_SALES", Label = "Sales Trends", Icon = "trending-up", Route = "/reports/sales-trends", ParentId = Guid.NewGuid(), Position = 3 },
            new Menu { Id = Guid.NewGuid(), Name = "REPORTS_TRACEABILITY", Label = "Traceability", Icon = "search", Route = "/reports/traceability", ParentId = Guid.NewGuid(), Position = 4 },

            // Admin children
            new Menu { Id = Guid.NewGuid(), Name = "ADMIN_USERS", Label = "Users", Icon = "users", Route = "/admin/users", ParentId = Guid.NewGuid(), Position = 0 },
            new Menu { Id = Guid.NewGuid(), Name = "ADMIN_ROLES", Label = "Roles & Permissions", Icon = "shield", Route = "/admin/roles", ParentId = Guid.NewGuid(), Position = 1 },
            new Menu { Id = Guid.NewGuid(), Name = "ADMIN_MODULES", Label = "Modules", Icon = "puzzle", Route = "/admin/modules", ParentId = Guid.NewGuid(), Position = 2 },
            new Menu { Id = Guid.NewGuid(), Name = "ADMIN_SETTINGS", Label = "Tenant Settings", Icon = "sliders", Route = "/admin/settings", ParentId = Guid.NewGuid(), Position = 3 },
            new Menu { Id = Guid.NewGuid(), Name = "ADMIN_AUDIT", Label = "Audit Logs", Icon = "scroll", Route = "/admin/audit", ParentId = Guid.NewGuid(), Position = 4 }
        );
    }
}