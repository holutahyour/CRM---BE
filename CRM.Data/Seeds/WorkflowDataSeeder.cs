using CRM.Base.Common;
using CRM.Domain.Enums.Workflow;

namespace CRM.Data.Seeds;

/// <summary>
/// Seeds the approval workflow specified in the Role Allocation Document (§5.1): MANAGER /
/// MANAGER (ACCESS) are the <b>first approval layer</b> for all item requests and requisitions
/// raised by their downliners, after which requests escalate to the <b>higher administrative
/// level</b> (ADMIN). Modelled as two active, role-based templates — one per <see cref="WorkflowType"/> —
/// on the SYSTEM tenant, each with the two ordered steps: Department Manager → Administrative Approval.
///
/// Routing is role-based; the per-step <c>UserId</c> "line-manager option" is left null here and can be
/// pinned to a specific manager later via the Approval Workflows UI (PUT /workflows/{id}/steps).
/// CREATED/INFERRED caveats are recorded in docs/IMPORT-NOTES.md (G-W1..G-W4). Idempotent — guarded on
/// an existing active template per type.
/// </summary>
public class WorkflowDataSeeder
{
    private readonly ApplicationDbContext _context;

    public WorkflowDataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> InitializeAsync()
    {
        var tenant = await _context.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Code == "SYSTEM");
        var tenantId = tenant?.Id ?? Guid.Empty;

        var managerRoleId = await _context.Roles.IgnoreQueryFilters()
            .Where(r => r.Code == "MANAGER").Select(r => (Guid?)r.Id).FirstOrDefaultAsync();
        var adminRoleId = await _context.Roles.IgnoreQueryFilters()
            .Where(r => r.Code == "ADMIN").Select(r => (Guid?)r.Id).FirstOrDefaultAsync();

        if (managerRoleId is null || adminRoleId is null)
            return "Workflow templates skipped: MANAGER and/or ADMIN role not found.";

        var created = 0;
        created += await EnsureTemplateAsync(tenantId, WorkflowType.Requisition, "Requisition Approval", managerRoleId.Value, adminRoleId.Value);
        created += await EnsureTemplateAsync(tenantId, WorkflowType.ItemRequest, "Item Request Approval", managerRoleId.Value, adminRoleId.Value);

        return $"Workflow templates seeded: {created} created (Requisition + Item Request → Manager then Admin).";
    }

    private async Task<int> EnsureTemplateAsync(Guid tenantId, WorkflowType type, string name, Guid managerRoleId, Guid adminRoleId)
    {
        var exists = await _context.WorkflowTemplates.IgnoreQueryFilters()
            .AnyAsync(t => t.WorkflowType == type && t.IsActive && t.TenantId == tenantId);
        if (exists) return 0;

        var template = new WorkflowTemplate
        {
            Id = Guid.NewGuid(),
            Code = RandomGenerator.RandomString(10),
            WorkflowType = type,
            Name = name,
            IsActive = true,
            TenantId = tenantId,
            CreatedBy = "SYSTEM",
            CreatedOn = DateTime.UtcNow,
            LastModifiedBy = "SYSTEM",
            LastModifiedOn = DateTime.UtcNow,
        };

        // Step 1 — first approval layer: the requester's manager (doc §5.1, "no bypass").
        template.Steps.Add(NewStep(tenantId, template.Id, 1, "Department Manager", managerRoleId));
        // Step 2 — higher administrative level, final sign-off (doc §5.1/§2).
        template.Steps.Add(NewStep(tenantId, template.Id, 2, "Administrative Approval", adminRoleId));

        _context.WorkflowTemplates.Add(template);
        await _context.SaveChangesAsync();
        return 1;
    }

    private static WorkflowStep NewStep(Guid tenantId, Guid templateId, int order, string stepName, Guid roleId) => new()
    {
        Id = Guid.NewGuid(),
        Code = RandomGenerator.RandomString(10),
        WorkflowTemplateId = templateId,
        StepOrder = order,
        StepName = stepName,
        RoleId = roleId,
        UserId = null, // role-based; pin a specific manager via the UI to use the line-manager option
        TenantId = tenantId,
        CreatedBy = "SYSTEM",
        CreatedOn = DateTime.UtcNow,
        LastModifiedBy = "SYSTEM",
        LastModifiedOn = DateTime.UtcNow,
    };
}
