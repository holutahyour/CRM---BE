namespace CRM.Data.Seeds;

public class Seeder
{
    private readonly ApplicationDbContext _context;

    public Seeder(ApplicationDbContext context)
    {
        _context = context;
    }
    public void Intialize()
    {
        // Tenants
        if (!_context.Tenants.IgnoreQueryFilters().Any(t => t.Code == "SYSTEM"))
        {
            var tenant = new Tenant
            {
                Id = Guid.Empty,
                Name = "System",
                Code = "SYSTEM",
                SubscriptionStatus = SubscriptionStatus.Active,
            };

            _context.Tenants.Add(tenant);
            _context.SaveChanges();

            // Seed roles. The six documented Eupepsia access tiers (SUPER_ADMIN, ADMIN, MANAGER,
            // MANAGER_ACCESS, DEPT_USER, ARTISAN) plus the pre-existing generic STAFF role.
            var superAdminRole    = new Role { TenantId = tenant.Id, Name = "Super Administrator", Code = "SUPER_ADMIN", IsSystem = true };
            var adminRole         = new Role { TenantId = tenant.Id, Name = "Administrator", Code = "ADMIN", IsSystem = true };
            var managerRole       = new Role { TenantId = tenant.Id, Name = "Manager", Code = "MANAGER", IsSystem = true };
            var managerAccessRole = new Role { TenantId = tenant.Id, Name = "Manager (Cross-Dept Access)", Code = "MANAGER_ACCESS", IsSystem = true };
            var deptUserRole      = new Role { TenantId = tenant.Id, Name = "Department User", Code = "DEPT_USER", IsSystem = true };
            var artisanRole       = new Role { TenantId = tenant.Id, Name = "User (Artisan)", Code = "ARTISAN", IsSystem = true };
            var staffRole         = new Role { TenantId = tenant.Id, Name = "Staff", Code = "STAFF", IsSystem = true };

            _context.Roles.AddRange([superAdminRole, adminRole, managerRole, managerAccessRole, deptUserRole, artisanRole, staffRole]);

            var allPermissions = _context.Permissions.ToList();

            void Assign(Role role, IEnumerable<string> codes)
            {
                foreach (var code in codes.Distinct())
                {
                    var perm = allPermissions.FirstOrDefault(p => p.Code == code);
                    if (perm != null)
                        _context.RolePermissions.Add(new RolePermission { TenantId = tenant.Id, RoleId = role.Id, PermissionId = perm.Id });
                }
            }

            // SUPER ADMIN — everything (incl. user/role/settings/module/menu management).
            foreach (var perm in allPermissions)
                _context.RolePermissions.Add(new RolePermission { TenantId = tenant.Id, RoleId = superAdminRole.Id, PermissionId = perm.Id });

            // ADMIN — full operational visibility but NOT the admin-management actions reserved
            // for SUPER ADMIN per the Role Allocation doc (onboarding users, settings, etc.).
            // (CREATED boundary, flagged G-R4 in docs/IMPORT-NOTES.md.)
            Assign(adminRole, allPermissions.Select(p => p.Code).Where(c => !AdminManagementPermissions.Contains(c)));

            // MANAGER / MANAGER (ACCESS) — department oversight + first-level approvals.
            string[] managerPerms = [
                Permissions.ItemsView, Permissions.ItemsCreate, Permissions.ItemsEdit,
                Permissions.CategoriesView, Permissions.BatchesView,
                Permissions.StockView, Permissions.ScannerUse,
                Permissions.LocationsView, Permissions.TransfersView, Permissions.TransfersCreate,
                Permissions.SuppliersView, Permissions.SuppliersManage, Permissions.SuppliersPerformanceView,
                Permissions.PurchaseOrdersView, Permissions.PurchaseOrdersCreate, Permissions.PurchaseOrdersApprove,
                Permissions.SalesOrdersView, Permissions.SalesOrdersCreate, Permissions.SalesOrdersFulfill,
                Permissions.RequisitionsView, Permissions.RequisitionsApprove,
                Permissions.ItemRequestsView,
                Permissions.IncidentsView, Permissions.IncidentsCreate, Permissions.IncidentsResolve,
                Permissions.DepartmentsView, Permissions.MonthlyReportsView,
                Permissions.ReportsView, Permissions.ReportsExport,
                Permissions.UsersView
            ];
            Assign(managerRole, managerPerms);
            // MANAGER (ACCESS) carries the same permissions; the doc's "cross-departmental" scope
            // is NOT enforced by the system today (flagged G-R3).
            Assign(managerAccessRole, managerPerms);

            // DEPARTMENT USER — standard own-department user; can raise requests/requisitions.
            string[] deptUserPerms = [
                Permissions.ItemsView, Permissions.CategoriesView, Permissions.BatchesView,
                Permissions.StockView, Permissions.ScannerUse, Permissions.LocationsView,
                Permissions.SalesOrdersView, Permissions.SalesOrdersCreate,
                Permissions.RequisitionsView, Permissions.RequisitionsCreate,
                Permissions.ItemRequestsView, Permissions.ItemRequestsCreate,
                Permissions.IncidentsView, Permissions.IncidentsCreate
            ];
            Assign(deptUserRole, deptUserPerms);

            // USER (ARTISAN) — minimal operational access for field/artisan staff.
            string[] artisanPerms = [
                Permissions.ItemsView, Permissions.StockView, Permissions.ItemRequestsCreate
            ];
            Assign(artisanRole, artisanPerms);

            // STAFF — pre-existing generic limited role (kept for backwards compatibility).
            string[] staffPerms = [
                Permissions.ItemsView, Permissions.CategoriesView, Permissions.BatchesView,
                Permissions.StockView, Permissions.ScannerUse, Permissions.LocationsView,
                Permissions.SalesOrdersView, Permissions.SalesOrdersCreate
            ];
            Assign(staffRole, staffPerms);

            // Activate default modules
            var defaultModules = _context.Modules.Where(m => m.Code == "CORE" || m.Code == "INVENTORY");
            foreach (var mod in defaultModules)
                _context.TenantModules.Add(new TenantModule { TenantId = tenant.Id, ModuleId = mod.Id, IsActive = true, ActivatedAt = DateTime.UtcNow });

            _context.SaveChanges();
        }

        // Always sync: keep SUPER_ADMIN/ADMIN role permissions current. This handles new
        // permissions added via migrations after the initial seeding (the block above only
        // runs once, so this catch-all keeps the high-privilege roles up to date).
        SyncAdminRolePermissions();

        //Core

        var genders = GenderSeedData.GenerateGenderData();

        if (!_context.Genders.Any())
        {
            _context.Genders.AddRange(genders);
        }

        var countries = CountrySeedData.GenerateCountryData();

        if (!_context.Countries.Any())
        {
            var uniqueCountries = countries
                .GroupBy(c => c.CountryCode)
                .Select(g => g.First())
                .ToList();

            foreach (var country in uniqueCountries)
                country.Code = $"COUNTRY-{country.CountryCode}";

            _context.Countries.AddRange(uniqueCountries);
            _context.SaveChanges();
        }

        var states = StateSeedData.GenerateStateData();

        if (!_context.States.Any())
        {
            var uniqueStates = states
                .GroupBy(c => c.Abbreviation)
                .Select(g => g.First())
                .ToList();

            foreach (var state in uniqueStates)
                state.Code = $"STATE-{state.Abbreviation}";

            _context.States.AddRange(uniqueStates);
            _context.SaveChanges();
        }

        var ethnicities = EthnicitySeedData.GenerateEthnicityData();

        if (!_context.Ethnicities.Any())
        {
            _context.Ethnicities.AddRange(ethnicities);
        }

        var parameterDefinitions = ParameterDefinitionSeedData.GenerateParameterDefinitionData();

        if (!_context.ParameterDefinitions.Any())
        {
            _context.ParameterDefinitions.AddRange(parameterDefinitions);
        }

        _context.SaveChanges();

        // Operational + inventory data for Eupepsia / Soilless Farm Lab (idempotent, guarded).
        // Departments are also exposed via POST /seed-operational-data and inventory via
        // POST /seed-inventory-data, but running them here means --seed / dev-startup covers all.
        new OperationalDataSeeder(_context).InitializeAsync().GetAwaiter().GetResult();
        new InventoryDataSeeder(_context).InitializeAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Admin-management permission codes that are reserved for SUPER_ADMIN and withheld from
    /// ADMIN, per the Role Allocation document (only Super Admin/ICT onboards users, configures
    /// settings, etc.). CREATED boundary — see docs/IMPORT-NOTES.md (G-R4).
    /// </summary>
    private static readonly HashSet<string> AdminManagementPermissions = new(StringComparer.OrdinalIgnoreCase)
    {
        Permissions.UsersManage, Permissions.RolesManage, Permissions.SettingsManage,
        Permissions.ModulesManage, Permissions.MenusManage
    };

    /// <summary>
    /// Idempotently keeps high-privilege roles current with the permission catalogue. Safe to
    /// call on every startup — it only INSERTs missing links, never duplicates or removes.
    /// SUPER_ADMIN receives every permission; ADMIN receives every permission EXCEPT the
    /// admin-management set reserved for SUPER_ADMIN.
    /// </summary>
    private void SyncAdminRolePermissions()
    {
        var allPermissions = _context.Permissions
            .IgnoreQueryFilters()
            .Where(p => !p.IsDeleted)
            .ToList();

        bool changed = false;

        void Sync(string roleCode, List<Permission> targetPerms)
        {
            var roles = _context.Roles
                .IgnoreQueryFilters()
                .Where(r => r.Code == roleCode && !r.IsDeleted)
                .ToList();

            foreach (var role in roles)
            {
                var existingPermIds = _context.RolePermissions
                    .IgnoreQueryFilters()
                    .Where(rp => rp.RoleId == role.Id && !rp.IsDeleted)
                    .Select(rp => rp.PermissionId)
                    .ToHashSet();

                foreach (var perm in targetPerms.Where(p => !existingPermIds.Contains(p.Id)))
                {
                    _context.RolePermissions.Add(new RolePermission
                    {
                        TenantId = role.TenantId,
                        RoleId = role.Id,
                        PermissionId = perm.Id
                    });
                    changed = true;
                }
            }
        }

        Sync("SUPER_ADMIN", allPermissions);
        Sync("ADMIN", allPermissions.Where(p => !AdminManagementPermissions.Contains(p.Code)).ToList());

        if (changed)
            _context.SaveChanges();
    }
}
