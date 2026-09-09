using System.Security.Cryptography;
using System.Text;

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
    private static readonly Guid MenuReqId = new Guid("a0a0a0a0-0a0a-0a0a-0a0a-a0a0a0a0a0a0");
    private static readonly Guid MenuItemReqId = new Guid("b0b0b0b0-0b0b-0b0b-0b0b-b0b0b0b0b0b0");
    private static readonly Guid MenuMonRepId = new Guid("c0c0c0c0-0c0c-0c0c-0c0c-c0c0c0c0c0c0");
    private static readonly Guid MenuInvId = new Guid("22222222-2222-2222-2222-222222222222");
    private static readonly Guid MenuMonId = new Guid("d1d1d1d1-1d1d-1d1d-1d1d-d1d1d1d1d1d2");
    private static readonly Guid MenuIncId = new Guid("d0d0d0d0-0d0d-0d0d-0d0d-d0d0d0d0d0d0");
    private static readonly Guid MenuUsrMgmtId = new Guid("e0e0e0e0-0e0e-0e0e-0e0e-e0e0e0e0e0e0");
    private static readonly Guid MenuDeptId = new Guid("f0f0f0f0-0f0f-0f0f-0f0f-f0f0f0f0f0f0");
    private static readonly Guid MenuAdmId = new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7");
    private static readonly Guid MenuApprovalWorkflowsId = new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1");
    private static readonly Guid MenuOpsId = new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2");
    private static readonly Guid MenuSalesId = new Guid("c3c3c3c3-3c3c-3c3c-3c3c-c3c3c3c3c3c3");

    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Menu>().HasData(
            // Root menus — ordered to match the sidebar screenshot
            new Menu { Id = MenuDashId, Name = "DASHBOARD", Label = "Dashboard", Icon = "layout-dashboard", Route = "/dashboard", Position = 0, ModuleCode = ModuleCodes.Core },
            new Menu { Id = MenuReqId, Name = "REQUISITIONS", Label = "Requisitions", Icon = "file-text", Route = "/requisitions", Position = 1, ModuleCode = ModuleCodes.Core },
            new Menu { Id = MenuItemReqId, Name = "ITEM_REQUESTS", Label = "Item Requests", Icon = "package", Route = "/item-requests", Position = 2, ModuleCode = ModuleCodes.Core },
            new Menu { Id = MenuMonRepId, Name = "MONTHLY_REPORTS", Label = "Monthly Reports", Icon = "bar-chart-2", Route = "/reports/monthly", Position = 3, ModuleCode = ModuleCodes.Reports },
            new Menu { Id = MenuInvId, Name = "INVENTORY", Label = "Inventory", Icon = "archive", Position = 4, ModuleCode = ModuleCodes.Inventory },
            new Menu { Id = MenuMonId, Name = "MONTHLY_REPORTS", Label = "Monthly Reports", Icon = "chart-column", Route = "/monthly-reports", Position = 5, ModuleCode = ModuleCodes.Core },
            new Menu { Id = MenuIncId, Name = "INCIDENT_REPORTS", Label = "Incident Reports", Icon = "alert-circle", Route = "/incident-reports", Position = 6, ModuleCode = ModuleCodes.Core },
            new Menu { Id = MenuUsrMgmtId, Name = "USER_MANAGEMENT", Label = "User Management", Icon = "users", Route = "/admin/users", Position = 7, ModuleCode = ModuleCodes.Core },
            new Menu { Id = MenuDeptId, Name = "DEPARTMENTS", Label = "Departments", Icon = "building-2", Route = "/departments", Position = 8, ModuleCode = ModuleCodes.Core },
            new Menu { Id = MenuOpsId, Name = "OPERATIONS", Label = "Operations", Icon = "home", Route = "/operations", Position = 9, ModuleCode = ModuleCodes.Core },
            new Menu { Id = MenuAdmId, Name = "ADMIN", Label = "Administration", Icon = "settings", Position = 10, ModuleCode = ModuleCodes.Core },
            new Menu { Id = MenuApprovalWorkflowsId, Name = "APPROVAL_WORKFLOWS", Label = "Approval Workflows", Icon = "git-branch", Route = "/approval-workflows", Position = 11, ModuleCode = ModuleCodes.Core },
            new Menu { Id = MenuSalesId, Name = "SALES", Label = "Sales", Icon = "trending-up", Route = "/sales", Position = 12, ModuleCode = ModuleCodes.Core },

            // Inventory children (unchanged)
            new Menu { Id = new Guid("b1b1b1b1-1b1b-1b1b-1b1b-b1b1b1b1b1b1"), Name = "INVENTORY_ITEMS", Label = "Items", Icon = "box", Route = "/inventory/items", ParentId = MenuInvId, Position = 0 },
            new Menu { Id = new Guid("b2b2b2b2-2b2b-2b2b-2b2b-b2b2b2b2b2b2"), Name = "INVENTORY_CATEGORIES", Label = "Categories", Icon = "tags", Route = "/inventory/categories", ParentId = MenuInvId, Position = 1 },
            //new Menu { Id = new Guid("b3b3b3b3-3b3b-3b3b-3b3b-b3b3b3b3b3b3"), Name = "INVENTORY_BATCHES",    Label = "Batches & Lots",  Icon = "layers",        Route = "/inventory/batches",     ParentId = MenuInvId, Position = 2 },
            //new Menu { Id = new Guid("b4b4b4b4-4b4b-4b4b-4b4b-b4b4b4b4b4b4"), Name = "INVENTORY_STOCK",      Label = "Stock Levels",    Icon = "bar-chart",     Route = "/inventory/stock",       ParentId = MenuInvId, Position = 3 },
            //new Menu { Id = new Guid("b5b5b5b5-5b5b-5b5b-5b5b-b5b5b5b5b5b5"), Name = "INVENTORY_SCANNER",    Label = "Barcode Scanner", Icon = "scan",          Route = "/inventory/scanner",     ParentId = MenuInvId, Position = 4, ModuleCode = ModuleCodes.Barcode },

            // Administration children (unchanged)
            new Menu { Id = new Guid("2f2f2f2f-2f2f-2f2f-2f2f-2f2f2f2f2f2f"), Name = "ADMIN_ROLES", Label = "Roles & Permissions", Icon = "shield", Route = "/admin/roles", ParentId = MenuAdmId, Position = 0 },
            new Menu { Id = new Guid("3f3f3f3f-3f3f-3f3f-3f3f-3f3f3f3f3f3f"), Name = "ADMIN_MODULES", Label = "Modules", Icon = "puzzle", Route = "/admin/modules", ParentId = MenuAdmId, Position = 1 },
            //new Menu { Id = new Guid("4f4f4f4f-4f4f-4f4f-4f4f-4f4f4f4f4f4f"), Name = "ADMIN_SETTINGS",Label = "Tenant Settings",     Icon = "sliders",      Route = "/admin/settings",  ParentId = MenuAdmId, Position = 2 },
            new Menu { Id = new Guid("5f5f5f5f-5f5f-5f5f-5f5f-5f5f5f5f5f5f"), Name = "ADMIN_AUDIT", Label = "Audit Logs", Icon = "scroll", Route = "/admin/audit", ParentId = MenuAdmId, Position = 3 },
            new Menu { Id = new Guid("6f6f6f6f-6f6f-6f6f-6f6f-6f6f6f6f6f6f"), Name = "ADMIN_MENUS", Label = "Menu Management", Icon = "layout-grid", Route = "/admin/menus", ParentId = MenuAdmId, Position = 4 }
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

        // Dashboard
        AddMapping(new Guid("d1d1d1d1-1d1d-1d1d-1d1d-d1d1d1d1d1d1"), Permissions.UsersView); // Everyone who can log in sees dashboard

        // Requisitions
        AddMapping(new Guid("a0a0a0a0-0a0a-0a0a-0a0a-a0a0a0a0a0a0"), Permissions.RequisitionsView);

        // Item Requests
        AddMapping(new Guid("b0b0b0b0-0b0b-0b0b-0b0b-b0b0b0b0b0b0"), Permissions.ItemRequestsView);

        // Monthly Reports
        AddMapping(new Guid("c0c0c0c0-0c0c-0c0c-0c0c-c0c0c0c0c0c0"), Permissions.MonthlyReportsView);

        // Inventory
        AddMapping(new Guid("22222222-2222-2222-2222-222222222222"), Permissions.ItemsView);
        AddMapping(new Guid("b1b1b1b1-1b1b-1b1b-1b1b-b1b1b1b1b1b1"), Permissions.ItemsView);
        AddMapping(new Guid("b2b2b2b2-2b2b-2b2b-2b2b-b2b2b2b2b2b2"), Permissions.CategoriesView);
        AddMapping(new Guid("b3b3b3b3-3b3b-3b3b-3b3b-b3b3b3b3b3b3"), Permissions.BatchesView);
        AddMapping(new Guid("b4b4b4b4-4b4b-4b4b-4b4b-b4b4b4b4b4b4"), Permissions.StockView);
        AddMapping(new Guid("b5b5b5b5-5b5b-5b5b-5b5b-b5b5b5b5b5b5"), Permissions.ScannerUse);

        // Incident Reports
        AddMapping(new Guid("d0d0d0d0-0d0d-0d0d-0d0d-d0d0d0d0d0d0"), Permissions.IncidentsView);

        // User Management
        AddMapping(new Guid("e0e0e0e0-0e0e-0e0e-0e0e-e0e0e0e0e0e0"), Permissions.UsersView);
        AddMapping(new Guid("e0e0e0e0-0e0e-0e0e-0e0e-e0e0e0e0e0e0"), Permissions.UsersManage);

        // Departments
        AddMapping(new Guid("f0f0f0f0-0f0f-0f0f-0f0f-f0f0f0f0f0f0"), Permissions.DepartmentsView);

        // Administration
        AddMapping(new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"), Permissions.RolesView);
        AddMapping(new Guid("2f2f2f2f-2f2f-2f2f-2f2f-2f2f2f2f2f2f"), Permissions.RolesView);
        AddMapping(new Guid("2f2f2f2f-2f2f-2f2f-2f2f-2f2f2f2f2f2f"), Permissions.RolesManage);
        AddMapping(new Guid("3f3f3f3f-3f3f-3f3f-3f3f-3f3f3f3f3f3f"), Permissions.ModulesView);
        AddMapping(new Guid("3f3f3f3f-3f3f-3f3f-3f3f-3f3f3f3f3f3f"), Permissions.ModulesManage);
        AddMapping(new Guid("4f4f4f4f-4f4f-4f4f-4f4f-4f4f4f4f4f4f"), Permissions.SettingsView);
        AddMapping(new Guid("4f4f4f4f-4f4f-4f4f-4f4f-4f4f4f4f4f4f"), Permissions.SettingsManage);
        AddMapping(new Guid("5f5f5f5f-5f5f-5f5f-5f5f-5f5f5f5f5f5f"), Permissions.AuditView);
        AddMapping(new Guid("6f6f6f6f-6f6f-6f6f-6f6f-6f6f6f6f6f6f"), Permissions.MenusView);
        AddMapping(new Guid("6f6f6f6f-6f6f-6f6f-6f6f-6f6f6f6f6f6f"), Permissions.MenusManage);

        // Operations
        AddMapping(new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"), Permissions.OperationsView);
        AddMapping(new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"), Permissions.OperationsManage);

        // Sales Department
        AddMapping(new Guid("c3c3c3c3-3c3c-3c3c-3c3c-c3c3c3c3c3c3"), Permissions.SalesDeptView);
        AddMapping(new Guid("c3c3c3c3-3c3c-3c3c-3c3c-c3c3c3c3c3c3"), Permissions.SalesDeptManage);

        // Approval Workflows
        AddMapping(new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1"), Permissions.WorkflowTemplatesView);
        AddMapping(new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1"), Permissions.WorkflowTemplatesManage);

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
