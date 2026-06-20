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

        // Departments — Eupepsia / Soilless Farm Lab.
        // NOTE: the source Role Allocation document has NO department master list; these
        // departments are INFERRED by grouping each staff member's Position, and StaffCount
        // is COMPUTED from the 47-person list. Budget/ProjectsCount/PercentOfTotal are NOT in
        // the source and are seeded as 0 (flagged in departments.json / docs/IMPORT-NOTES.md).
        // Guard with IgnoreQueryFilters + explicit tenant predicate: during seeding the
        // ITenantProvider resolves TenantId to Guid.Empty (no HTTP context), which does NOT
        // match the SYSTEM tenant's real (DB-generated) Id, so a filtered AnyAsync() would never
        // see existing rows and would duplicate on every run.
        if (!await _context.Set<Department>().IgnoreQueryFilters().AnyAsync(d => d.TenantId == defaultTenantId))
        {
            var departments = EupepsiaSeedData.Departments().Select(d => new Department
            {
                Id = Guid.NewGuid(),
                Code = d.Code,
                Name = d.Name,
                Description = d.Description,
                StaffCount = d.StaffCount,
                ProjectsCount = d.ProjectsCount,
                Budget = d.Budget,
                PercentOfTotal = d.PercentOfTotal,
                TenantId = defaultTenantId,
                CreatedBy = "SYSTEM",
                CreatedOn = DateTime.UtcNow,
                LastModifiedBy = "SYSTEM",
                LastModifiedOn = DateTime.UtcNow,
            }).ToList();
            await _context.Set<Department>().AddRangeAsync(departments);
        }

        await _context.SaveChangesAsync();
    }

}
