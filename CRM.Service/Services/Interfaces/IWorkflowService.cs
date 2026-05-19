using CRM.Domain.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Enums.Workflow;

namespace CRM.Services.Interfaces;

public interface IWorkflowService
{
    Task<WorkflowTemplate> CreateTemplateAsync(CreateWorkflowTemplateRequest request, Guid tenantId);
    Task UpsertStepsAsync(Guid templateId, List<WorkflowStepRequest> steps, Guid tenantId);
    Task<WorkflowTemplate?> GetActiveTemplateAsync(WorkflowType workflowType, Guid tenantId);
    Task<List<WorkflowTemplate>> GetAllTemplatesAsync(Guid tenantId);
}
