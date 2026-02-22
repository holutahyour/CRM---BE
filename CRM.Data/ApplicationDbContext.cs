using CRM.Base.Common.Domain.Entities;
using CRM.Base.Domain.Entities;
using CRM.Data.Seeds.Access_Control;
using CRM.Domain.Entities.Inventory;
using CRM.Domain.Entities.Orders;
using CRM.Services.Services.Interfaces.Common;

namespace CRM.Data
{
    public class ApplicationDbContext : Base.Common.Repositories.ApplicationDbContext
    {
        private readonly ITenantProvider _tenantProvider;

        public ApplicationDbContext() : base(new DbContextOptionsBuilder<ApplicationDbContext>().Options)
        {
            _tenantProvider = new DesignTimeTenantProvider();
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantProvider tenantProvider) : base(options)
        {
            _tenantProvider = tenantProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Design-time fallback
                optionsBuilder.UseSqlServer("Server=localhost,1433;Database=crm_dev;User ID=sa;Password=YourPassword123!;MultipleActiveResultSets=true;TrustServerCertificate=True");
            }
        }

        private class DesignTimeTenantProvider : ITenantProvider
        {
            public Guid TenantId => Guid.Empty;
            public string? UserId => null;
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

            // Apply global filters
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Multi-tenancy filter
                if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(ApplyTenantFilter),
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                        .MakeGenericMethod(entityType.ClrType);

                    method.Invoke(this, [modelBuilder]);
                }
                // Soft-delete filter (only if not already handled by tenant filter which includes it)
                else if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(ApplySoftDeleteFilter),
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                        .MakeGenericMethod(entityType.ClrType);

                    method.Invoke(this, [modelBuilder]);
                }
            }

            ModuleSeedData.Seed(modelBuilder);
            PermissionSeedData.Seed(modelBuilder);
            MenuSeedData.Seed(modelBuilder);
            //RolePermissionSeedData.Seed(modelBuilder);

        }

        private void ApplyTenantFilter<T>(ModelBuilder modelBuilder) where T : class, ITenantEntity, ISoftDelete
        {
            modelBuilder.Entity<T>().HasQueryFilter(e =>
                e.TenantId == _tenantProvider.TenantId && !e.IsDeleted);
        }

        private void ApplySoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : class, ISoftDelete
        {
            modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
