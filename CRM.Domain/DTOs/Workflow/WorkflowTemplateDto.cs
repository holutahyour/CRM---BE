using CRM.Domain.Enums.Workflow;

namespace CRM.Domain.DTOs;

public class WorkflowStepRequest
{
    public string StepName { get; set; } = string.Empty;
    public int StepOrder { get; set; }
    public Guid RoleId { get; set; }
    public Guid? UserId { get; set; }
}

public class CreateWorkflowTemplateRequest
{
    public WorkflowType WorkflowType { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<WorkflowStepRequest> Steps { get; set; } = new();
}

public class UpdateWorkflowTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public List<WorkflowStepRequest> Steps { get; set; } = new();
}

public class WorkflowStepResponse
{
    public Guid Id { get; set; }
    public int StepOrder { get; set; }
    public string StepName { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
}

public class WorkflowTemplateResponse
{
    public Guid Id { get; set; }
    public WorkflowType WorkflowType { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<WorkflowStepResponse> Steps { get; set; } = new();
}
