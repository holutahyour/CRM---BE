using CRM.Domain.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Domain.Enums.Workflow;
using CRM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Services.Implementations.Workflow;

public class ApprovalService : IApprovalService
{
    private readonly CRM.Data.ApplicationDbContext _db;

    public ApprovalService(CRM.Data.ApplicationDbContext db) => _db = db;

    public async Task ApproveAsync(WorkflowType workflowType, Guid entityId, Guid userId, Guid tenantId)
    {
        var (template, currentStep) = await LoadContextAsync(workflowType, entityId, userId, tenantId);

        // Record the approval
        _db.ApprovalRecords.Add(new ApprovalRecord
        {
            Id = Guid.NewGuid(),
            WorkflowType = workflowType,
            EntityId = entityId,
            StepOrder = currentStep.StepOrder,
            StepName = currentStep.StepName,
            ActionedBy = userId,
            ActionedOn = DateTime.UtcNow,
            Status = ApprovalStatus.Approved,
            TenantId = tenantId
        });

        var orderedSteps = template.Steps.OrderBy(s => s.StepOrder).ToList();
        bool isLastStep = currentStep.StepOrder >= orderedSteps.Max(s => s.StepOrder);

        if (workflowType == WorkflowType.Requisition)
        {
            var entity = await _db.Requisitions
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Id == entityId);

            if (entity is null)
                throw new InvalidOperationException($"Requisition {entityId} not found.");

            if (isLastStep)
                entity.Status = RequisitionStatus.Approved;
            else
                entity.CurrentStepOrder = currentStep.StepOrder + 1;
        }
        else if (workflowType == WorkflowType.ItemRequest)
        {
            var entity = await _db.ItemRequests
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Id == entityId);

            if (entity is null)
                throw new InvalidOperationException($"ItemRequest {entityId} not found.");

            if (isLastStep)
                entity.Status = ItemRequestStatus.Approved;
            else
                entity.CurrentStepOrder = currentStep.StepOrder + 1;
        }

        await _db.SaveChangesAsync();
    }

    public async Task RejectAsync(WorkflowType workflowType, Guid entityId, Guid userId, string notes, Guid tenantId)
    {
        var (_, currentStep) = await LoadContextAsync(workflowType, entityId, userId, tenantId);

        // Record the rejection
        _db.ApprovalRecords.Add(new ApprovalRecord
        {
            Id = Guid.NewGuid(),
            WorkflowType = workflowType,
            EntityId = entityId,
            StepOrder = currentStep.StepOrder,
            StepName = currentStep.StepName,
            ActionedBy = userId,
            ActionedOn = DateTime.UtcNow,
            Status = ApprovalStatus.Rejected,
            Notes = notes,
            TenantId = tenantId
        });

        if (workflowType == WorkflowType.Requisition)
        {
            var entity = await _db.Requisitions
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Id == entityId);

            if (entity is null)
                throw new InvalidOperationException($"Requisition {entityId} not found.");

            entity.Status = RequisitionStatus.Rejected;
        }
        else if (workflowType == WorkflowType.ItemRequest)
        {
            var entity = await _db.ItemRequests
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Id == entityId);

            if (entity is null)
                throw new InvalidOperationException($"ItemRequest {entityId} not found.");

            entity.Status = ItemRequestStatus.Rejected;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<ApprovalHistoryResponse> GetHistoryAsync(WorkflowType workflowType, Guid entityId, Guid tenantId)
    {
        var template = await _db.WorkflowTemplates
            .IgnoreQueryFilters()
            .Include(t => t.Steps.OrderBy(s => s.StepOrder))
            .FirstOrDefaultAsync(t => t.WorkflowType == workflowType && t.IsActive && t.TenantId == tenantId);

        var records = await _db.ApprovalRecords
            .IgnoreQueryFilters()
            .Where(r => r.WorkflowType == workflowType && r.EntityId == entityId && r.TenantId == tenantId)
            .OrderBy(r => r.StepOrder)
            .ToListAsync();

        int currentStepOrder = 1;
        if (workflowType == WorkflowType.Requisition)
        {
            var entity = await _db.Requisitions
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Id == entityId);
            currentStepOrder = entity?.CurrentStepOrder ?? 1;
        }
        else if (workflowType == WorkflowType.ItemRequest)
        {
            var entity = await _db.ItemRequests
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Id == entityId);
            currentStepOrder = entity?.CurrentStepOrder ?? 1;
        }

        var steps = template?.Steps
            .Select(s => new WorkflowStepResponse
            {
                Id = s.Id,
                StepOrder = s.StepOrder,
                StepName = s.StepName,
                RoleId = s.RoleId,
                UserId = s.UserId
            })
            .ToList() ?? new List<WorkflowStepResponse>();

        var recordResponses = records
            .Select(r => new ApprovalRecordResponse
            {
                Id = r.Id,
                StepOrder = r.StepOrder,
                StepName = r.StepName,
                ActionedByName = r.ActionedBy.ToString(),
                ActionedOn = r.ActionedOn,
                Status = r.Status,
                Notes = r.Notes
            })
            .ToList();

        return new ApprovalHistoryResponse
        {
            WorkflowSteps = steps,
            Records = recordResponses,
            CurrentStepOrder = currentStepOrder
        };
    }

    /// <summary>
    /// Loads and validates the workflow context for an approval/rejection action.
    /// Throws <see cref="InvalidOperationException"/> if no active template exists.
    /// Throws <see cref="UnauthorizedAccessException"/> if the user is not authorized for the current step.
    /// </summary>
    private async Task<(WorkflowTemplate template, WorkflowStep currentStep)> LoadContextAsync(
        WorkflowType workflowType, Guid entityId, Guid userId, Guid tenantId)
    {
        // Find the active template for this workflow type — IgnoreQueryFilters to bypass tenant filter in test contexts
        var template = await _db.WorkflowTemplates
            .IgnoreQueryFilters()
            .Include(t => t.Steps.OrderBy(s => s.StepOrder))
            .FirstOrDefaultAsync(t => t.WorkflowType == workflowType && t.IsActive && t.TenantId == tenantId);

        if (template is null)
            throw new InvalidOperationException("No active workflow template found for this workflow type.");

        // Determine the current step order from the entity
        int currentStepOrder;
        if (workflowType == WorkflowType.Requisition)
        {
            var entity = await _db.Requisitions
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Id == entityId);
            if (entity is null)
                throw new InvalidOperationException($"Requisition {entityId} not found.");
            currentStepOrder = entity.CurrentStepOrder;
        }
        else if (workflowType == WorkflowType.ItemRequest)
        {
            var entity = await _db.ItemRequests
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Id == entityId);
            if (entity is null)
                throw new InvalidOperationException($"ItemRequest {entityId} not found.");
            currentStepOrder = entity.CurrentStepOrder;
        }
        else
        {
            throw new InvalidOperationException($"Unsupported workflow type: {workflowType}");
        }

        var currentStep = template.Steps
            .FirstOrDefault(s => s.StepOrder == currentStepOrder)
            ?? throw new InvalidOperationException($"No step found with order {currentStepOrder}.");

        // Authorization: user must either be the named user override OR have the required role
        bool isUserOverride = currentStep.UserId.HasValue && currentStep.UserId.Value == userId;

        if (!isUserOverride)
        {
            var hasRole = await _db.UserRoles
                .IgnoreQueryFilters()
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == currentStep.RoleId && ur.TenantId == tenantId);

            if (!hasRole)
                throw new UnauthorizedAccessException(
                    $"User {userId} is not authorized to action step '{currentStep.StepName}'.");
        }

        return (template, currentStep);
    }
}
