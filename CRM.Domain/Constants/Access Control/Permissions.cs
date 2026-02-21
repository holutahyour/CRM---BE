namespace CRM.Domain.Constants;

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
