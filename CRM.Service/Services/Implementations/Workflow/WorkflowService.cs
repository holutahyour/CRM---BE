using CRM.Domain.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Enums.Workflow;
using CRM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Services.Implementations.Workflow;

public class WorkflowService : IWorkflowService
{
    private readonly CRM.Data.ApplicationDbContext _db;

    public WorkflowService(CRM.Data.ApplicationDbContext db) => _db = db;

    public async Task<WorkflowTemplate> CreateTemplateAsync(CreateWorkflowTemplateRequest request, Guid tenantId)
    {
        var template = new WorkflowTemplate
        {
            Id = Guid.NewGuid(),
            WorkflowType = request.WorkflowType,
            Name = request.Name,
            IsActive = true
            // TenantId is set automatically by ApplicationDbContext.SaveChangesAsync
        };

        foreach (var s in request.Steps)
        {
            template.Steps.Add(new WorkflowStep
            {
                Id = Guid.NewGuid(),
                WorkflowTemplateId = template.Id,
                StepOrder = s.StepOrder,
                StepName = s.StepName,
                RoleId = s.RoleId,
                UserId = s.UserId
                // TenantId is set automatically by ApplicationDbContext.SaveChangesAsync
            });
        }

        _db.WorkflowTemplates.Add(template);
        await _db.SaveChangesAsync();
        return template;
    }

    public async Task UpsertStepsAsync(Guid templateId, List<WorkflowStepRequest> steps, Guid tenantId)
    {
        // Use IgnoreQueryFilters to find existing steps regardless of tenant/soft-delete filters
        var existing = _db.WorkflowSteps
            .IgnoreQueryFilters()
            .Where(s => s.WorkflowTemplateId == templateId);

        _db.WorkflowSteps.RemoveRange(existing);

        foreach (var s in steps)
        {
            _db.WorkflowSteps.Add(new WorkflowStep
            {
                Id = Guid.NewGuid(),
                WorkflowTemplateId = templateId,
                StepOrder = s.StepOrder,
                StepName = s.StepName,
                RoleId = s.RoleId,
                UserId = s.UserId
                // TenantId is set automatically by ApplicationDbContext.SaveChangesAsync
            });
        }

        await _db.SaveChangesAsync();
    }

    public async Task<WorkflowTemplate?> GetActiveTemplateAsync(WorkflowType workflowType, Guid tenantId)
        => await _db.WorkflowTemplates
            .Include(t => t.Steps.OrderBy(s => s.StepOrder))
            .FirstOrDefaultAsync(t => t.WorkflowType == workflowType && t.IsActive);

    public async Task<List<WorkflowTemplate>> GetAllTemplatesAsync(Guid tenantId)
        => await _db.WorkflowTemplates
            .Include(t => t.Steps.OrderBy(s => s.StepOrder))
            .ToListAsync();
}
