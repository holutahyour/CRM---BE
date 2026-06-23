using System.Text;
using CRM.Base.Common.Domain.Entities;
using CRM.Data;
using CRM.Domain.DTOs.Core;
using CRM.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace CRM.Tests.Services;

public class AuditLogServiceTests
{
    private readonly Guid _tenantA = Guid.NewGuid();
    private readonly Guid _tenantB = Guid.NewGuid();

    private ApplicationDbContext Seed()
    {
        var db = TestDbContext.Create(_tenantA);

        db.Set<AuditLog>().AddRange(
            new AuditLog { Id = 1, ActionType = "Create", EntityName = "Item", UserId = "u1", Timestamp = DateTime.UtcNow.AddMinutes(-3), IpAddress = "127.0.0.1", TenantId = _tenantA },
            new AuditLog { Id = 2, ActionType = "Update", EntityName = "Item", UserId = "u1", Timestamp = DateTime.UtcNow.AddMinutes(-2), IpAddress = "127.0.0.1", TenantId = _tenantA },
            new AuditLog { Id = 3, ActionType = "Create", EntityName = "Role", UserId = "u9", Timestamp = DateTime.UtcNow.AddMinutes(-1), IpAddress = "127.0.0.1", TenantId = _tenantB }
        );
        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task GetAllAsync_scopes_to_current_tenant()
    {
        var db = Seed();
        var service = new AuditLogService(db);

        var result = await service.GetAllAsync(new AuditLogFilter(), _tenantA, "http://x/audit");

        result.IsSuccess.Should().BeTrue();
        result.Content.Should().HaveCount(2);
        result.Content.Should().OnlyContain(a => a.TenantId == _tenantA);
    }

    [Fact]
    public async Task GetAllAsync_allTenants_returns_every_tenant()
    {
        var db = Seed();
        var service = new AuditLogService(db);

        var result = await service.GetAllAsync(new AuditLogFilter(AllTenants: true), _tenantA, "http://x/audit");

        result.IsSuccess.Should().BeTrue();
        result.Content.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_filters_by_actionType()
    {
        var db = Seed();
        var service = new AuditLogService(db);

        var result = await service.GetAllAsync(new AuditLogFilter(ActionType: "Update"), _tenantA, "http://x/audit");

        result.Content.Should().ContainSingle();
        result.Content[0].ActionType.Should().Be("Update");
    }

    [Fact]
    public async Task GetAllAsync_orders_newest_first()
    {
        var db = Seed();
        var service = new AuditLogService(db);

        var result = await service.GetAllAsync(new AuditLogFilter(), _tenantA, "http://x/audit");

        result.Content.Should().BeInDescendingOrder(a => a.Timestamp);
    }

    [Fact]
    public async Task ExportCsvAsync_returns_csv_with_header_and_rows()
    {
        var db = Seed();
        var service = new AuditLogService(db);

        var result = await service.ExportCsvAsync(new AuditLogFilter(), _tenantA);

        result.IsSuccess.Should().BeTrue();
        var text = Encoding.UTF8.GetString(result.Content);
        text.Should().StartWith("Timestamp,ActionType,EntityName,UserId,IpAddress,TenantId,AdditionalInfo");
        text.Should().Contain("Update");
        text.Should().NotContain("Role"); // tenantB row excluded
    }
}
