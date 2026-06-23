namespace CRM.Domain.Constants;

/// <summary>
/// All fine-grained permission codes used throughout the system.
/// These are seeded into the database and checked by authorization handlers.
/// </summary>
public static class Permissions
{
    // Inventory
    public const string ItemsView = "inventory.items.view";

    // Requisitions
    public const string RequisitionsView = "requisitions.view";
    public const string RequisitionsCreate = "requisitions.create";
    public const string RequisitionsApprove = "requisitions.approve";

    // Item Requests
    public const string ItemRequestsView = "itemrequests.view";
    public const string ItemRequestsCreate = "itemrequests.create";

    // Incidents
    public const string IncidentsView = "incidents.view";
    public const string IncidentsCreate = "incidents.create";
    public const string IncidentsResolve = "incidents.resolve";

    // Departments
    public const string DepartmentsView = "departments.view";
    public const string DepartmentsManage = "departments.manage";

    // Monthly Reports
    public const string MonthlyReportsView = "reports.monthly.view";

    public const string ItemsCreate = "inventory.items.create";
    public const string ItemsEdit = "inventory.items.edit";
    public const string ItemsDelete = "inventory.items.delete";

    // Categories
    public const string CategoriesView = "inventory.categories.view";
    public const string CategoriesManage = "inventory.categories.manage";

    // Batches
    public const string BatchesView = "inventory.batches.view";
    public const string BatchesManage = "inventory.batches.manage";

    // Stock
    public const string StockView = "inventory.stock.view";
    public const string ScannerUse = "inventory.scanner.use";

    // Locations
    public const string LocationsView = "inventory.locations.view";
    public const string LocationsManage = "inventory.locations.manage";
    public const string TransfersView = "inventory.transfers.view";
    public const string TransfersCreate = "inventory.transfers.create";

    // Suppliers
    public const string SuppliersView = "suppliers.view";
    public const string SuppliersManage = "suppliers.manage";
    public const string SuppliersPerformanceView = "suppliers.performance.view";

    // Purchase Orders
    public const string PurchaseOrdersView = "orders.purchase.view";
    public const string PurchaseOrdersCreate = "orders.purchase.create";
    public const string PurchaseOrdersApprove = "orders.purchase.approve";

    // Sales Orders
    public const string SalesOrdersView = "orders.sales.view";
    public const string SalesOrdersCreate = "orders.sales.create";
    public const string SalesOrdersFulfill = "orders.sales.fulfill";

    // E-Commerce
    public const string EcommerceManage = "orders.ecommerce.manage";

    // Reports
    public const string ReportsView = "reports.view";
    public const string ReportsExport = "reports.export";

    // Administration
    public const string UsersView = "admin.users.view";
    public const string UsersManage = "admin.users.manage";
    public const string RolesView = "admin.roles.view";
    public const string RolesManage = "admin.roles.manage";
    public const string SettingsView = "admin.settings.view";
    public const string SettingsManage = "admin.settings.manage";
    public const string AuditView = "admin.audit.view";
    public const string ModulesView = "admin.modules.view";
    public const string ModulesManage = "admin.modules.manage";
    public const string MenusView = "admin.menus.view";
    public const string MenusManage = "admin.menus.manage";

    /// <summary>Super-admin: manage global catalogs (module catalog, menu tree) and cross-tenant views.</summary>
    public const string SystemManage = "admin.system.manage";

    // Workflow Templates
    public const string WorkflowTemplatesView = "workflows.templates.view";
    public const string WorkflowTemplatesManage = "workflows.templates.manage";

    /// <summary>Returns all permission codes as a flat list (useful for seeding).</summary>
    public static IReadOnlyList<string> All => typeof(Permissions)
        .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
        .Where(f => f.FieldType == typeof(string))
        .Select(f => (string)f.GetValue(null)!)
        .ToList();
}
