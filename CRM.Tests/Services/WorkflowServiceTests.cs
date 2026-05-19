using CRM.Data;
using CRM.Domain.DTOs;
using CRM.Domain.Enums.Workflow;
using CRM.Domain.Entities;
using CRM.Tests.Helpers;
using CRM.Services.Implementations.Workflow;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CRM.Tests.Services;

public class WorkflowServiceTests
{
    [Fact]
    public async Task CreateTemplate_SavesStepsInCorrectOrder()
    {
        var db = TestDbContext.Create();
        var service = new WorkflowService(db);
        var tenantId = Guid.NewGuid();

        var request = new CreateWorkflowTemplateRequest
        {
            WorkflowType = WorkflowType.Requisition,
            Name = "Requisition Chain",
            Steps = new List<WorkflowStepRequest>
            {
                new() { StepName = "Supervisor", StepOrder = 1, RoleId = Guid.NewGuid() },
                new() { StepName = "Finance",    StepOrder = 2, RoleId = Guid.NewGuid() },
            }
        };

        await service.CreateTemplateAsync(request, tenantId);

        var saved = db.WorkflowTemplates
            .Include(t => t.Steps)
            .FirstOrDefault(x => x.WorkflowType == WorkflowType.Requisition);
        saved.Should().NotBeNull();
        saved!.Steps.Should().HaveCount(2);
        saved.Steps.OrderBy(s => s.StepOrder).First().StepName.Should().Be("Supervisor");
    }

    [Fact]
    public async Task UpsertSteps_ReplacesAllExistingSteps()
    {
        var db = TestDbContext.Create();
        var service = new WorkflowService(db);
        var tenantId = Guid.NewGuid();

        var request = new CreateWorkflowTemplateRequest
        {
            WorkflowType = WorkflowType.Requisition,
            Name = "Chain",
            Steps = new List<WorkflowStepRequest>
            {
                new() { StepName = "Old Step", StepOrder = 1, RoleId = Guid.NewGuid() }
            }
        };
        var template = await service.CreateTemplateAsync(request, tenantId);

        var newSteps = new List<WorkflowStepRequest>
        {
            new() { StepName = "New Step A", StepOrder = 1, RoleId = Guid.NewGuid() },
            new() { StepName = "New Step B", StepOrder = 2, RoleId = Guid.NewGuid() },
        };
        await service.UpsertStepsAsync(template.Id, newSteps, tenantId);

        var updated = db.WorkflowTemplates
            .Include(t => t.Steps)
            .First(x => x.Id == template.Id);
        updated.Steps.Should().HaveCount(2);
        updated.Steps.Should().NotContain(s => s.StepName == "Old Step");
    }
}
