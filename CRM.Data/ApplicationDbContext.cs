using CRM.Base.Common.Domain.Entities;
using CRM.Base.Domain.Entities;
using CRM.Data.Seeds.Access_Control;
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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var tenantId = _tenantProvider?.TenantId ?? Guid.Empty;
            var userId = _tenantProvider?.UserId ?? "SYSTEM";

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = userId;
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = userId;
                        entry.Entity.LastModifiedOn = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = userId;
                        entry.Entity.LastModifiedOn = DateTime.UtcNow;
                        break;
                }
            }

            if (tenantId != Guid.Empty)
            {
                foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
                {
                    if (entry.State == EntityState.Added)
                    {
                        if (entry.Entity.TenantId == Guid.Empty)
                        {
                            entry.Entity.TenantId = tenantId;
                        }
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
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

        // Organization
        public DbSet<Department> Departments => Set<Department>();

        // Requisitions
        public DbSet<Activity> Activities => Set<Activity>();
        public DbSet<DashboardSummary> DashboardSummaries => Set<DashboardSummary>();
        public DbSet<Incident> Incidents => Set<Incident>();
        public DbSet<ItemRequest> ItemRequests => Set<ItemRequest>();
        public DbSet<MonthlyReport> MonthlyReports => Set<MonthlyReport>();
        public DbSet<Requisition> Requisitions => Set<Requisition>();

        // Workflow
        public DbSet<WorkflowTemplate> WorkflowTemplates => Set<WorkflowTemplate>();
        public DbSet<WorkflowStep> WorkflowSteps => Set<WorkflowStep>();
        public DbSet<ApprovalRecord> ApprovalRecords => Set<ApprovalRecord>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DashboardSummary>().HasNoKey().ToView(null);

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
            MenuPermissionSeedData.Seed(modelBuilder);
            RoleSeedData.Seed(modelBuilder);
            RolePermissionSeedData.Seed(modelBuilder);
            SuperAdminRoleSeedData.Seed(modelBuilder);

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
