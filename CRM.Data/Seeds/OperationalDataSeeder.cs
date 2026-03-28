namespace CRM.Data.Seeds;

public class OperationalDataSeeder
{
    private readonly ApplicationDbContext _context;
    private Guid defaultTenantId = default;

    public OperationalDataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task InitializeAsync()
    {
        // Operational Seed Data
        var defaultTenant = await _context.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Code == "SYSTEM");
        if (defaultTenant != null) defaultTenantId = defaultTenant.Id;

        if (!await _context.Set<Department>().AnyAsync())
        {
            var departments = new List<Department>
            {
                new Department { Id = Guid.NewGuid(), Code = "FIN", Name = "Finance", Description = "Finance and Accounting", StaffCount = 10, ProjectsCount = 2, Budget = 5000000, PercentOfTotal = 20, TenantId = defaultTenantId},
                new Department { Id = Guid.NewGuid(), Code = "HR", Name = "Human Resources", Description = "Human Resources Management", StaffCount = 5, ProjectsCount = 1, Budget = 2000000, PercentOfTotal = 10, TenantId = defaultTenantId},
                new Department { Id = Guid.NewGuid(), Code = "IT", Name = "IT Support", Description = "Information Technology", StaffCount = 15, ProjectsCount = 5, Budget = 10000000, PercentOfTotal = 30, TenantId = defaultTenantId},
                new Department { Id = Guid.NewGuid(), Code = "MKT", Name = "Marketing", Description = "Marketing and Sales", StaffCount = 8, ProjectsCount = 4, Budget = 4000000, PercentOfTotal = 15, TenantId = defaultTenantId},
                new Department { Id = Guid.NewGuid(), Code = "OPS", Name = "Operations", Description = "General Operations", StaffCount = 12, ProjectsCount = 3, Budget = 6000000, PercentOfTotal = 25, TenantId = defaultTenantId},
            };
            await _context.Set<Department>().AddRangeAsync(departments);
        }

        await _context.SaveChangesAsync();
    }

}
