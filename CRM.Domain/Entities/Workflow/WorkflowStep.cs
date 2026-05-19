using CRM.Base.Domain.Entities;

namespace CRM.Domain.Entities;

public class WorkflowStep : TenantEntity<Guid>
{
    public Guid WorkflowTemplateId { get; set; }
    public int StepOrder { get; set; }
    public string StepName { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public Guid? UserId { get; set; }

    public WorkflowTemplate Template { get; set; } = null!;
    public Role Role { get; set; } = null!;
    public User? User { get; set; }
}
