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

            // Seed default roles
            var adminRole = new Role { TenantId = tenant.Id, Name = "Administrator", Code = "ADMIN", IsSystem = true };
            var managerRole = new Role { TenantId = tenant.Id, Name = "Manager", Code = "MANAGER", IsSystem = true };
            var staffRole = new Role { TenantId = tenant.Id, Name = "Staff", Code = "STAFF", IsSystem = true };

            _context.Roles.AddRange([adminRole, managerRole, staffRole]);

            // Assign permissions to Admin
            var allPermissions = _context.Permissions.ToList();
            foreach (var perm in allPermissions)
                _context.RolePermissions.Add(new RolePermission { TenantId = tenant.Id, RoleId = adminRole.Id, PermissionId = perm.Id });

            // Assign permissions to Manager
            string[] managerPerms = [
                Permissions.ItemsView, Permissions.ItemsCreate, Permissions.ItemsEdit,
                Permissions.CategoriesView, Permissions.BatchesView,
                Permissions.StockView, Permissions.ScannerUse,
                Permissions.LocationsView, Permissions.TransfersView, Permissions.TransfersCreate,
                Permissions.SuppliersView, Permissions.SuppliersManage, Permissions.SuppliersPerformanceView,
                Permissions.PurchaseOrdersView, Permissions.PurchaseOrdersCreate, Permissions.PurchaseOrdersApprove,
                Permissions.SalesOrdersView, Permissions.SalesOrdersCreate, Permissions.SalesOrdersFulfill,
                Permissions.ReportsView, Permissions.ReportsExport,
                Permissions.UsersView
            ];

            foreach (var code in managerPerms)
            {
                var perm = allPermissions.FirstOrDefault(p => p.Code == code);
                if (perm != null)
                    _context.RolePermissions.Add(new RolePermission { TenantId = tenant.Id, RoleId = managerRole.Id, PermissionId = perm.Id });
            }

            // Assign permissions to Staff
            string[] staffPerms = [
                Permissions.ItemsView,
                Permissions.CategoriesView,
                Permissions.BatchesView,
                Permissions.StockView,
                Permissions.ScannerUse,
                Permissions.LocationsView,
                Permissions.SalesOrdersView, Permissions.SalesOrdersCreate
            ];

            foreach (var code in staffPerms)
            {
                var perm = allPermissions.FirstOrDefault(p => p.Code == code);
                if (perm != null)
                    _context.RolePermissions.Add(new RolePermission { TenantId = tenant.Id, RoleId = staffRole.Id, PermissionId = perm.Id });
            }

            // Activate default modules
            var defaultModules = _context.Modules.Where(m => m.Code == "CORE" || m.Code == "INVENTORY");
            foreach (var mod in defaultModules)
                _context.TenantModules.Add(new TenantModule { TenantId = tenant.Id, ModuleId = mod.Id, IsActive = true, ActivatedAt = DateTime.UtcNow });

            _context.SaveChanges();
        }

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
    }
}
