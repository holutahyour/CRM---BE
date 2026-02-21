using CRM.Base.Common.Domain.Entities;
using CRM.Base.Domain.Entities;
using CRM.Domain.Entities.Inventory;
using CRM.Domain.Entities.Orders;
using CRM.Services.Services.Interfaces.Common;

namespace CRM.Data
{
    public class ApplicationDbContext : Base.Common.Repositories.ApplicationDbContext
    {
        private readonly DbContextOptions<ApplicationDbContext> options;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantProvider tenantProvider) : base(options)
        {
            this.options = options;
        }

        //Core
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Ethnicity> Ethnicities { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        //Core/Access Control
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

        // Inventory
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<ItemLocation> ItemLocations => Set<ItemLocation>();
        public DbSet<Batch> Batches => Set<Batch>();
        public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

        // Orders
        public DbSet<Vendor> Vendors => Set<Vendor>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
        public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
        public DbSet<SalesOrderItem> SalesOrderItems => Set<SalesOrderItem>();

        //Parameter
        public DbSet<ParameterDefinition> ParameterDefinitions { get; set; }
        public DbSet<ParameterValue> ParameterValues { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all IEntityTypeConfiguration classes from this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Apply global tenant query filters
            //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            //{
            //    if (typeof(TenantEntity<>).IsAssignableFrom(entityType.ClrType))
            //    {
            //        var method = typeof(ApplicationDbContext)
            //            .GetMethod(nameof(ApplyTenantFilter),
            //                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            //            .MakeGenericMethod(entityType.ClrType);

            //        method.Invoke(this, [modelBuilder]);
            //    }
            //}

            // Apply soft-delete filter for all BaseEntity types
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity<>).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(ApplySoftDeleteFilter),
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                        .MakeGenericMethod(entityType.ClrType);

                    method.Invoke(this, [modelBuilder]);
                }
            }
        }

        //private void ApplyTenantFilter<T, J>(ModelBuilder modelBuilder) where T : TenantEntity<J>
        //{
        //    modelBuilder.Entity<T>().HasQueryFilter(e =>
        //        e.TenantId == tenantProvider.TenantId && !e.IsDeleted);
        //}

        private void ApplySoftDeleteFilter<T, J>(ModelBuilder modelBuilder) where T : BaseEntity<J>
        {
            // Only apply if not already filtered by tenant (which includes IsDeleted)
            if (!typeof(TenantEntity<>).IsAssignableFrom(typeof(T)))
            {
                modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
            }
        }
    }
}
