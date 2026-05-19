using CRM.Domain.DTOs;
using CRM.Domain.Enums.Workflow;

namespace CRM.Services.Interfaces;

public interface IApprovalService
{
    Task ApproveAsync(WorkflowType workflowType, Guid entityId, Guid userId, Guid tenantId);
    Task RejectAsync(WorkflowType workflowType, Guid entityId, Guid userId, string notes, Guid tenantId);
    Task<ApprovalHistoryResponse> GetHistoryAsync(WorkflowType workflowType, Guid entityId, Guid tenantId);
}
