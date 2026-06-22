using CRM.Data;
using CRM.Data.Seeds;
using CRM.Domain.Entities;
using CRM.Domain.Enums.Workflow;
using CRM.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CRM.Tests.Services;

public class WorkflowDataSeederTests
{
    private static async Task<(ApplicationDbContext db, Guid tenantId)> SetupAsync(bool withManagerRole = true)
    {
        var tenantId = Guid.NewGuid();
        var db = TestDbContext.Create(tenantId);

        db.Tenants.Add(new Tenant { Id = tenantId, Name = "System", Code = "SYSTEM" });
        db.Roles.Add(new Role { Id = Guid.NewGuid(), Name = "Administrator", Code = "ADMIN", TenantId = tenantId, IsSystem = true });
        if (withManagerRole)
            db.Roles.Add(new Role { Id = Guid.NewGuid(), Name = "Manager", Code = "MANAGER", TenantId = tenantId, IsSystem = true });

        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        return (db, tenantId);
    }

    [Fact]
    public async Task Seeds_two_templates_each_with_manager_then_admin_steps()
    {
        var (db, tenantId) = await SetupAsync();

        await new WorkflowDataSeeder(db).InitializeAsync();

        db.ChangeTracker.Clear();
        var templates = await db.WorkflowTemplates.IgnoreQueryFilters()
            .Include(t => t.Steps)
            .Where(t => t.TenantId == tenantId)
            .ToListAsync();

        templates.Should().HaveCount(2);
        templates.Select(t => t.WorkflowType).Should()
            .BeEquivalentTo(new[] { WorkflowType.Requisition, WorkflowType.ItemRequest });

        var roleCodeById = await db.Roles.IgnoreQueryFilters().ToDictionaryAsync(r => r.Id, r => r.Code);

        foreach (var t in templates)
        {
            t.IsActive.Should().BeTrue();
            var steps = t.Steps.OrderBy(s => s.StepOrder).ToList();
            steps.Should().HaveCount(2);

            steps[0].StepOrder.Should().Be(1);
            steps[0].StepName.Should().Be("Department Manager");
            roleCodeById[steps[0].RoleId].Should().Be("MANAGER"); // first approval layer (doc §5.1)
            steps[0].UserId.Should().BeNull();                    // role-based; line-manager option un-set

            steps[1].StepOrder.Should().Be(2);
            steps[1].StepName.Should().Be("Administrative Approval");
            roleCodeById[steps[1].RoleId].Should().Be("ADMIN");   // higher administrative level (final)
        }
    }

    [Fact]
    public async Task Is_idempotent_across_reruns()
    {
        var (db, tenantId) = await SetupAsync();

        await new WorkflowDataSeeder(db).InitializeAsync();
        await new WorkflowDataSeeder(db).InitializeAsync();

        (await db.WorkflowTemplates.IgnoreQueryFilters().CountAsync(t => t.TenantId == tenantId)).Should().Be(2);
        (await db.WorkflowSteps.IgnoreQueryFilters().CountAsync(s => s.TenantId == tenantId)).Should().Be(4);
    }

    [Fact]
    public async Task Skips_when_a_required_role_is_missing()
    {
        var (db, tenantId) = await SetupAsync(withManagerRole: false);

        var summary = await new WorkflowDataSeeder(db).InitializeAsync();

        summary.Should().Contain("skipped");
        (await db.WorkflowTemplates.IgnoreQueryFilters().CountAsync(t => t.TenantId == tenantId)).Should().Be(0);
    }
}
