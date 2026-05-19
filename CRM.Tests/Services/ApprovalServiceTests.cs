using CRM.Data;
using CRM.Domain.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Domain.Enums.Workflow;
using CRM.Services.Implementations.Workflow;
using CRM.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace CRM.Tests.Services;

public class ApprovalServiceTests
{
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _roleId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    private ApplicationDbContext BuildDb() => TestDbContext.Create(_tenantId);

    private async Task<(ApplicationDbContext db, Requisition req, WorkflowTemplate template)> SetupAsync(int totalSteps = 2)
    {
        var db = BuildDb();

        var template = new WorkflowTemplate
        {
            Id = Guid.NewGuid(),
            WorkflowType = WorkflowType.Requisition,
            Name = "Test Chain",
            IsActive = true,
            TenantId = _tenantId
        };

        for (int i = 1; i <= totalSteps; i++)
        {
            template.Steps.Add(new WorkflowStep
            {
                Id = Guid.NewGuid(),
                StepOrder = i,
                StepName = $"Step {i}",
                RoleId = _roleId,
                TenantId = _tenantId
            });
        }

        db.WorkflowTemplates.Add(template);

        // Seed the approver's role so the service can verify authorization
        db.UserRoles.Add(new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            RoleId = _roleId,
            TenantId = _tenantId
        });

        var req = new Requisition
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Amount = 100,
            Status = RequisitionStatus.Pending,
            CurrentStepOrder = 1,
            TenantId = _tenantId
        };

        db.Requisitions.Add(req);
        await db.SaveChangesAsync();
        return (db, req, template);
    }

    [Fact]
    public async Task Approve_AdvancesCurrentStepOrder()
    {
        var (db, req, _) = await SetupAsync(totalSteps: 2);
        var service = new ApprovalService(db);

        await service.ApproveAsync(WorkflowType.Requisition, req.Id, _userId, _tenantId);

        db.ChangeTracker.Clear();
        var updated = await db.Requisitions.FindAsync(req.Id);
        updated!.CurrentStepOrder.Should().Be(2);
        updated.Status.Should().Be(RequisitionStatus.Pending);
    }

    [Fact]
    public async Task Approve_LastStep_MarksEntityApproved()
    {
        var (db, req, _) = await SetupAsync(totalSteps: 1);
        var service = new ApprovalService(db);

        await service.ApproveAsync(WorkflowType.Requisition, req.Id, _userId, _tenantId);

        db.ChangeTracker.Clear();
        var updated = await db.Requisitions.FindAsync(req.Id);
        updated!.Status.Should().Be(RequisitionStatus.Approved);
    }

    [Fact]
    public async Task Reject_MarksEntityRejectedAndDoesNotAdvance()
    {
        var (db, req, _) = await SetupAsync(totalSteps: 2);
        var service = new ApprovalService(db);

        await service.RejectAsync(WorkflowType.Requisition, req.Id, _userId, "Budget exceeded", _tenantId);

        db.ChangeTracker.Clear();
        var updated = await db.Requisitions.FindAsync(req.Id);
        updated!.Status.Should().Be(RequisitionStatus.Rejected);
        updated.CurrentStepOrder.Should().Be(1);
    }

    [Fact]
    public async Task Approve_WrongRole_ThrowsUnauthorizedAccessException()
    {
        var (db, req, _) = await SetupAsync();
        var wrongUserId = Guid.NewGuid();
        var service = new ApprovalService(db);

        var act = async () => await service.ApproveAsync(WorkflowType.Requisition, req.Id, wrongUserId, _tenantId);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Approve_UserOverride_AllowsSpecificUser()
    {
        var db = BuildDb();
        var specificUserId = Guid.NewGuid();

        var template = new WorkflowTemplate
        {
            Id = Guid.NewGuid(),
            WorkflowType = WorkflowType.Requisition,
            Name = "Override Chain",
            IsActive = true,
            TenantId = _tenantId,
            Steps = new List<WorkflowStep>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    StepOrder = 1,
                    StepName = "Director",
                    RoleId = Guid.NewGuid(),
                    UserId = specificUserId,
                    TenantId = _tenantId
                }
            }
        };

        db.WorkflowTemplates.Add(template);

        var req = new Requisition
        {
            Id = Guid.NewGuid(),
            Title = "Override Test",
            Amount = 500,
            Status = RequisitionStatus.Pending,
            CurrentStepOrder = 1,
            TenantId = _tenantId
        };

        db.Requisitions.Add(req);
        await db.SaveChangesAsync();
        var service = new ApprovalService(db);

        var act = async () => await service.ApproveAsync(WorkflowType.Requisition, req.Id, specificUserId, _tenantId);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task GetHistory_ReturnsAllRecordsAndSteps()
    {
        var (db, req, template) = await SetupAsync();
        var service = new ApprovalService(db);
        await service.ApproveAsync(WorkflowType.Requisition, req.Id, _userId, _tenantId);

        var history = await service.GetHistoryAsync(WorkflowType.Requisition, req.Id, _tenantId);

        history.Records.Should().HaveCount(1);
        history.WorkflowSteps.Should().HaveCount(2);
        history.Records[0].Status.Should().Be(ApprovalStatus.Approved);
    }

    [Fact]
    public async Task Approve_NoActiveTemplate_ThrowsInvalidOperationException()
    {
        var db = BuildDb();
        var req = new Requisition
        {
            Id = Guid.NewGuid(),
            Title = "No Template",
            Amount = 100,
            Status = RequisitionStatus.Pending,
            CurrentStepOrder = 1,
            TenantId = _tenantId
        };

        db.Requisitions.Add(req);
        await db.SaveChangesAsync();
        var service = new ApprovalService(db);

        var act = async () => await service.ApproveAsync(WorkflowType.Requisition, req.Id, _userId, _tenantId);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*No active workflow template*");
    }
}
