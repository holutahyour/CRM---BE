using System.Security.Cryptography;
using System.Text;
using CRM.Domain.Constants;

namespace CRM.Data.Seeds.Access_Control;

public static class ModuleSeedData
{
    // Module Category IDs
    private static readonly Guid CatCoreId = new Guid("7f9e8a7d-1c3b-4e1a-8c2d-5b6a7d8e9f0a");
    private static readonly Guid CatOpsId = new Guid("8a9b0c1d-2e3f-4a5b-6c7d-8e9f0a1b2c3d");
    private static readonly Guid CatSupplyId = new Guid("9b0c1d2e-3f4a-5b6c-7d8e-9f0a1b2c3d4e");
    private static readonly Guid CatSalesId = new Guid("0c1d2e3f-4a5b-6c7d-8e9f-0a1b2c3d4e5f");
    private static readonly Guid CatIntelId = new Guid("1d2e3f4a-5b6c-7d8e-9f0a-1b2c3d4e5f6a");
    private static readonly Guid CatIntegId = new Guid("2e3f4a5b-6c7d-8e9f-0a1b-2c3d4e5f6a7b");

    // Module IDs
    private static readonly Guid ModCoreId = new Guid("3f4a5b6c-7d8e-9f0a-1b2c-3d4e5f6a7b8c");
    private static readonly Guid ModInvId = new Guid("4a5b6c7d-8e9f-0a1b-2c3d-4e5f6a7b8c9d");
    private static readonly Guid ModWhId = new Guid("5b6c7d8e-9f0a-1b2c-3d4e-5f6a7b8c9d0a");
    private static readonly Guid ModProcId = new Guid("6c7d8e9f-0a1b-2c3d-4e5f-6a7b8c9d0a1b");
    private static readonly Guid ModSalesId = new Guid("7d8e9f0a-1b2c-3d4e-5f6a-7b8c9d0a1b2c");
    private static readonly Guid ModSupId = new Guid("8e9f0a1b-2c3d-4e5f-6a7b-8c9d0a1b2c3d");
    private static readonly Guid ModRepId = new Guid("9f0a1b2c-3d4e-5f6a-7b8c-9a0b1c2d3e4f");
    private static readonly Guid ModEcomId = new Guid("0a1b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c5d");
    private static readonly Guid ModBarId = new Guid("1b2c3d4e-5f6a-7b8c-9d0e-1f2a3b4c5d6e");

    public static void Seed(ModelBuilder modelBuilder)
    {
        // Module Categories
        modelBuilder.Entity<ModuleCategory>().HasData(
            new ModuleCategory { Id = CatCoreId, Code = "CORE", Name = "Core", IsActive = true },
            new ModuleCategory { Id = CatOpsId, Code = "OPS", Name = "Operations", IsActive = true },
            new ModuleCategory { Id = CatSupplyId, Code = "SUPPLY", Name = "Supply Chain", IsActive = true },
            new ModuleCategory { Id = CatSalesId, Code = "SALES", Name = "Sales", IsActive = true },
            new ModuleCategory { Id = CatIntelId, Code = "INTEL", Name = "Intelligence", IsActive = true },
            new ModuleCategory { Id = CatIntegId, Code = "INTEG", Name = "Integrations", IsActive = true }
        );

        // Modules
        modelBuilder.Entity<Module>().HasData(
            new Module { Id = ModCoreId, Code = "CORE", Name = "Core & Administration", CategoryId = CatCoreId, Version = "1.0", IsActive = true },
            new Module { Id = ModInvId, Code = "INVENTORY", Name = "Inventory Management", CategoryId = CatOpsId, Version = "1.0", IsActive = true },
            new Module { Id = ModWhId, Code = "WAREHOUSE", Name = "Warehouse & Locations", CategoryId = CatOpsId, Version = "1.0", IsActive = true },
            new Module { Id = ModProcId, Code = "PROCUREMENT", Name = "Procurement", CategoryId = CatSupplyId, Version = "1.0", IsActive = true },
            new Module { Id = ModSalesId, Code = "SALES", Name = "Sales & Orders", CategoryId = CatSalesId, Version = "1.0", IsActive = true },
            new Module { Id = ModSupId, Code = "SUPPLIERS", Name = "Supplier Management", CategoryId = CatSupplyId, Version = "1.0", IsActive = true },
            new Module { Id = ModRepId, Code = "REPORTS", Name = "Reporting & Analytics", CategoryId = CatIntelId, Version = "1.0", IsActive = true },
            new Module { Id = ModEcomId, Code = "ECOMMERCE", Name = "E-Commerce Integration", CategoryId = CatIntegId, Version = "1.0", IsActive = true },
            new Module { Id = ModBarId, Code = "BARCODE", Name = "Barcode & Scanning", CategoryId = CatIntegId, Version = "1.0", IsActive = true }
        );
    }
}

public static class PermissionSeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var permissions = new List<Permission>();

        // Generate from Permissions constants class using reflection
        foreach (var code in Permissions.All.OrderBy(p => p))
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
                Id = CreateDeterministicGuid(code),
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

    private static Guid CreateDeterministicGuid(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return new Guid(hash);
        }
    }
}

public static class MenuSeedData
{
    // Root Menu IDs
    private static readonly Guid MenuDashId = new Guid("d1d1d1d1-1d1d-1d1d-1d1d-d1d1d1d1d1d1");
    private static readonly Guid MenuInvId = new Guid("22222222-2222-2222-2222-222222222222");
    private static readonly Guid MenuLocId = new Guid("33333333-3333-3333-3333-333333333333");
    private static readonly Guid MenuSupId = new Guid("44444444-4444-4444-4444-444444444444");
    private static readonly Guid MenuOrdId = new Guid("55555555-5555-5555-5555-555555555555");
    private static readonly Guid MenuRepId = new Guid("66666666-6666-6666-6666-666666666666");
    private static readonly Guid MenuAdmId = new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7");

    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Menu>().HasData(
            // Root menus
            new Menu { Id = MenuDashId, Name = "DASHBOARD", Label = "Dashboard", Icon = "layout-dashboard", Route = "/dashboard", Position = 0, ModuleCode = ModuleCodes.Core },
            new Menu { Id = MenuInvId, Name = "INVENTORY", Label = "Inventory", Icon = "package", Position = 1, ModuleCode = ModuleCodes.Inventory },
            new Menu { Id = MenuLocId, Name = "LOCATIONS", Label = "Locations", Icon = "map-pin", Position = 2, ModuleCode = ModuleCodes.Warehouse },
            new Menu { Id = MenuSupId, Name = "SUPPLIERS", Label = "Suppliers", Icon = "truck", Position = 3, ModuleCode = ModuleCodes.Suppliers },
            new Menu { Id = MenuOrdId, Name = "ORDERS", Label = "Orders", Icon = "clipboard-list", Position = 4 },
            new Menu { Id = MenuRepId, Name = "REPORTS", Label = "Reports", Icon = "bar-chart-3", Position = 5, ModuleCode = ModuleCodes.Reports },
            new Menu { Id = MenuAdmId, Name = "ADMIN", Label = "Administration", Icon = "settings", Position = 6, ModuleCode = ModuleCodes.Core },

            // Inventory children
            new Menu { Id = new Guid("b1b1b1b1-1b1b-1b1b-1b1b-b1b1b1b1b1b1"), Name = "INVENTORY_ITEMS", Label = "Items", Icon = "box", Route = "/inventory/items", ParentId = MenuInvId, Position = 0 },
            new Menu { Id = new Guid("b2b2b2b2-2b2b-2b2b-2b2b-b2b2b2b2b2b2"), Name = "INVENTORY_CATEGORIES", Label = "Categories", Icon = "tags", Route = "/inventory/categories", ParentId = MenuInvId, Position = 1 },
            new Menu { Id = new Guid("b3b3b3b3-3b3b-3b3b-3b3b-b3b3b3b3b3b3"), Name = "INVENTORY_BATCHES", Label = "Batches & Lots", Icon = "layers", Route = "/inventory/batches", ParentId = MenuInvId, Position = 2 },
            new Menu { Id = new Guid("b4b4b4b4-4b4b-4b4b-4b4b-b4b4b4b4b4b4"), Name = "INVENTORY_STOCK", Label = "Stock Levels", Icon = "bar-chart", Route = "/inventory/stock", ParentId = MenuInvId, Position = 3 },
            new Menu { Id = new Guid("b5b5b5b5-5b5b-5b5b-5b5b-b5b5b5b5b5b5"), Name = "INVENTORY_SCANNER", Label = "Barcode Scanner", Icon = "scan", Route = "/inventory/scanner", ParentId = MenuInvId, Position = 4, ModuleCode = ModuleCodes.Barcode },

            // Locations children
            new Menu { Id = new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1"), Name = "LOCATIONS_ALL", Label = "All Locations", Icon = "building", Route = "/locations", ParentId = MenuLocId, Position = 0 },
            new Menu { Id = new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"), Name = "LOCATIONS_TRANSFERS", Label = "Stock Transfers", Icon = "arrow-right-left", Route = "/locations/transfers", ParentId = MenuLocId, Position = 1 },

            // Suppliers children
            new Menu { Id = new Guid("d2d2d2d2-2d2d-2d2d-2d2d-d2d2d2d2d2d2"), Name = "SUPPLIERS_LIST", Label = "Supplier List", Icon = "users", Route = "/suppliers", ParentId = MenuSupId, Position = 0 },
            new Menu { Id = new Guid("d3d3d3d3-3d3d-3d3d-3d3d-d3d3d3d3d3d3"), Name = "SUPPLIERS_PERFORMANCE", Label = "Supplier Performance", Icon = "trending-up", Route = "/suppliers/performance", ParentId = MenuSupId, Position = 1 },

            // Orders children
            new Menu { Id = new Guid("e1e1e1e1-1e1e-1e1e-1e1e-e1e1e1e1e1e1"), Name = "ORDERS_PURCHASE", Label = "Purchase Orders", Icon = "shopping-cart", Route = "/orders/purchase", ParentId = MenuOrdId, Position = 0, ModuleCode = ModuleCodes.Procurement },
            new Menu { Id = new Guid("e2e2e2e2-2e2e-2e2e-2e2e-e2e2e2e2e2e2"), Name = "ORDERS_SALES", Label = "Sales Orders", Icon = "receipt", Route = "/orders/sales", ParentId = MenuOrdId, Position = 1, ModuleCode = ModuleCodes.Sales },
            new Menu { Id = new Guid("e3e3e3e3-3e3e-3e3e-3e3e-e3e3e3e3e3e3"), Name = "ORDERS_ECOM", Label = "E-Commerce Sync", Icon = "globe", Route = "/orders/ecommerce", ParentId = MenuOrdId, Position = 2, ModuleCode = ModuleCodes.ECommerce },

            // Reports children
            new Menu { Id = new Guid("f1f1f1f1-1f1f-1f1f-1f1f-f1f1f1f1f1f1"), Name = "REPORTS_STOCK", Label = "Stock Report", Icon = "file-bar-chart", Route = "/reports/stock", ParentId = MenuRepId, Position = 0 },
            new Menu { Id = new Guid("f2f2f2f2-2f2f-2f2f-2f2f-f2f2f2f2f2f2"), Name = "REPORTS_EXPIRATION", Label = "Expiration Alerts", Icon = "alert-triangle", Route = "/reports/expiration", ParentId = MenuRepId, Position = 1 },
            new Menu { Id = new Guid("f3f3f3f3-3f3f-3f3f-3f3f-f3f3f3f3f3f3"), Name = "REPORTS_MOVEMENTS", Label = "Movement History", Icon = "history", Route = "/reports/movements", ParentId = MenuRepId, Position = 2 },
            new Menu { Id = new Guid("f4f4f4f4-4f4f-4f4f-4f4f-f4f4f4f4f4f4"), Name = "REPORTS_SALES", Label = "Sales Trends", Icon = "trending-up", Route = "/reports/sales-trends", ParentId = MenuRepId, Position = 3 },
            new Menu { Id = new Guid("f5f5f5f5-5f5f-5f5f-5f5f-f5f5f5f5f5f5"), Name = "REPORTS_TRACEABILITY", Label = "Traceability", Icon = "search", Route = "/reports/traceability", ParentId = MenuRepId, Position = 4 },

            // Admin children
            new Menu { Id = new Guid("1f1f1f1f-1f1f-1f1f-1f1f-1f1f1f1f1f1f"), Name = "ADMIN_USERS", Label = "Users", Icon = "users", Route = "/admin/users", ParentId = MenuAdmId, Position = 0 },
            new Menu { Id = new Guid("2f2f2f2f-2f2f-2f2f-2f2f-2f2f2f2f2f2f"), Name = "ADMIN_ROLES", Label = "Roles & Permissions", Icon = "shield", Route = "/admin/roles", ParentId = MenuAdmId, Position = 1 },
            new Menu { Id = new Guid("3f3f3f3f-3f3f-3f3f-3f3f-3f3f3f3f3f3f"), Name = "ADMIN_MODULES", Label = "Modules", Icon = "puzzle", Route = "/admin/modules", ParentId = MenuAdmId, Position = 2 },
            new Menu { Id = new Guid("4f4f4f4f-4f4f-4f4f-4f4f-4f4f4f4f4f4f"), Name = "ADMIN_SETTINGS", Label = "Tenant Settings", Icon = "sliders", Route = "/admin/settings", ParentId = MenuAdmId, Position = 3 },
            new Menu { Id = new Guid("5f5f5f5f-5f5f-5f5f-5f5f-5f5f5f5f5f5f"), Name = "ADMIN_AUDIT", Label = "Audit Logs", Icon = "scroll", Route = "/admin/audit", ParentId = MenuAdmId, Position = 4 },
            new Menu { Id = new Guid("6f6f6f6f-6f6f-6f6f-6f6f-6f6f6f6f6f6f"), Name = "ADMIN_MENUS", Label = "Menu Management", Icon = "layout-grid", Route = "/admin/menus", ParentId = MenuAdmId, Position = 5 }
        );
    }
}

public static class MenuPermissionSeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var menuPermissions = new List<MenuPermission>();

        // We use deterministic GUIDs based on the MenuId + Permission Code
        void AddMapping(Guid menuId, string permissionCode)
        {
            menuPermissions.Add(new MenuPermission
            {
                Id = CreateDeterministicGuid($"{menuId}_{permissionCode}"),
                MenuId = menuId,
                PermissionId = CreateDeterministicGuid(permissionCode) // Needs to match Permission ID generation
            });
        }

        // Inventory Module
        AddMapping(new Guid("22222222-2222-2222-2222-222222222222"), Permissions.ItemsView); // Root Inventory
        AddMapping(new Guid("b1b1b1b1-1b1b-1b1b-1b1b-b1b1b1b1b1b1"), Permissions.ItemsView);
        AddMapping(new Guid("b2b2b2b2-2b2b-2b2b-2b2b-b2b2b2b2b2b2"), Permissions.CategoriesView);
        AddMapping(new Guid("b3b3b3b3-3b3b-3b3b-3b3b-b3b3b3b3b3b3"), Permissions.BatchesView);
        AddMapping(new Guid("b4b4b4b4-4b4b-4b4b-4b4b-b4b4b4b4b4b4"), Permissions.StockView);
        AddMapping(new Guid("b5b5b5b5-5b5b-5b5b-5b5b-b5b5b5b5b5b5"), Permissions.ScannerUse);

        // Locations Module
        AddMapping(new Guid("33333333-3333-3333-3333-333333333333"), Permissions.LocationsView); // Root Locations
        AddMapping(new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1"), Permissions.LocationsView);
        AddMapping(new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"), Permissions.TransfersView);
        AddMapping(new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"), Permissions.TransfersCreate);

        // Suppliers Module
        AddMapping(new Guid("44444444-4444-4444-4444-444444444444"), Permissions.SuppliersView); // Root Suppliers
        AddMapping(new Guid("d2d2d2d2-2d2d-2d2d-2d2d-d2d2d2d2d2d2"), Permissions.SuppliersView);
        AddMapping(new Guid("d3d3d3d3-3d3d-3d3d-3d3d-d3d3d3d3d3d3"), Permissions.SuppliersPerformanceView);

        // Orders Module
        AddMapping(new Guid("55555555-5555-5555-5555-555555555555"), Permissions.PurchaseOrdersView); // Root Orders
        AddMapping(new Guid("55555555-5555-5555-5555-555555555555"), Permissions.SalesOrdersView);   // Root Orders (allow sales too)
        AddMapping(new Guid("e1e1e1e1-1e1e-1e1e-1e1e-e1e1e1e1e1e1"), Permissions.PurchaseOrdersView);
        AddMapping(new Guid("e2e2e2e2-2e2e-2e2e-2e2e-e2e2e2e2e2e2"), Permissions.SalesOrdersView);
        AddMapping(new Guid("e3e3e3e3-3e3e-3e3e-3e3e-e3e3e3e3e3e3"), Permissions.EcommerceManage);

        // Reports Module
        AddMapping(new Guid("66666666-6666-6666-6666-666666666666"), Permissions.ReportsView); // Root Reports
        AddMapping(new Guid("f1f1f1f1-1f1f-1f1f-1f1f-f1f1f1f1f1f1"), Permissions.ReportsView);
        AddMapping(new Guid("f2f2f2f2-2f2f-2f2f-2f2f-f2f2f2f2f2f2"), Permissions.ReportsView);
        AddMapping(new Guid("f3f3f3f3-3f3f-3f3f-3f3f-f3f3f3f3f3f3"), Permissions.ReportsView);
        AddMapping(new Guid("f4f4f4f4-4f4f-4f4f-4f4f-f4f4f4f4f4f4"), Permissions.ReportsView);
        AddMapping(new Guid("f5f5f5f5-5f5f-5f5f-5f5f-f5f5f5f5f5f5"), Permissions.ReportsView);

        // Administration Module
        AddMapping(new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"), Permissions.UsersView);  // Root Admin
        AddMapping(new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"), Permissions.RolesView);  // Root Admin
        AddMapping(new Guid("1f1f1f1f-1f1f-1f1f-1f1f-1f1f1f1f1f1f"), Permissions.UsersView);
        AddMapping(new Guid("1f1f1f1f-1f1f-1f1f-1f1f-1f1f1f1f1f1f"), Permissions.UsersManage);
        AddMapping(new Guid("2f2f2f2f-2f2f-2f2f-2f2f-2f2f2f2f2f2f"), Permissions.RolesView);
        AddMapping(new Guid("2f2f2f2f-2f2f-2f2f-2f2f-2f2f2f2f2f2f"), Permissions.RolesManage);
        AddMapping(new Guid("3f3f3f3f-3f3f-3f3f-3f3f-3f3f3f3f3f3f"), Permissions.ModulesView);
        AddMapping(new Guid("3f3f3f3f-3f3f-3f3f-3f3f-3f3f3f3f3f3f"), Permissions.ModulesManage);
        AddMapping(new Guid("4f4f4f4f-4f4f-4f4f-4f4f-4f4f4f4f4f4f"), Permissions.SettingsView);
        AddMapping(new Guid("4f4f4f4f-4f4f-4f4f-4f4f-4f4f4f4f4f4f"), Permissions.SettingsManage);
        AddMapping(new Guid("5f5f5f5f-5f5f-5f5f-5f5f-5f5f5f5f5f5f"), Permissions.AuditView);
        AddMapping(new Guid("6f6f6f6f-6f6f-6f6f-6f6f-6f6f6f6f6f6f"), Permissions.MenusView);
        AddMapping(new Guid("6f6f6f6f-6f6f-6f6f-6f6f-6f6f6f6f6f6f"), Permissions.MenusManage);

        modelBuilder.Entity<MenuPermission>().HasData(menuPermissions.ToArray());
    }

    private static Guid CreateDeterministicGuid(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return new Guid(hash);
        }
    }
}
