using CRM.Base.Domain.Entities;
using CRM.Domain.Enums.Workflow;

namespace CRM.Domain.Entities;

public class WorkflowTemplate : TenantEntity<Guid>
{
    public WorkflowType WorkflowType { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
}
