using CRM.Base.Domain.Entities;
using CRM.Domain.Enums.Workflow;

namespace CRM.Domain.Entities;

public class ApprovalRecord : TenantEntity<Guid>
{
    public WorkflowType WorkflowType { get; set; }
    public Guid EntityId { get; set; }
    public int StepOrder { get; set; }
    public string StepName { get; set; } = string.Empty;
    public Guid ActionedBy { get; set; }
    public DateTime ActionedOn { get; set; }
    public ApprovalStatus Status { get; set; }
    public string? Notes { get; set; }

    public User ActionedByUser { get; set; } = null!;
}
