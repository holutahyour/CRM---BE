using CRM.Domain.Enums.Workflow;

namespace CRM.Domain.DTOs;

public class ApprovalHistoryResponse
{
    public List<WorkflowStepResponse> WorkflowSteps { get; set; } = new();
    public List<ApprovalRecordResponse> Records { get; set; } = new();
    public int CurrentStepOrder { get; set; }
}

public class ApprovalRecordResponse
{
    public Guid Id { get; set; }
    public int StepOrder { get; set; }
    public string StepName { get; set; } = string.Empty;
    public string ActionedByName { get; set; } = string.Empty;
    public DateTime ActionedOn { get; set; }
    public ApprovalStatus Status { get; set; }
    public string? Notes { get; set; }
}
