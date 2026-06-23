using AutoMapper;
using CRM.Base.Common.Domain.Entities;
using CRM.Base.Repositories.Implementations;
using CRM.Data;
using CRM.Domain.DTOs.Core;
using CRM.Domain.Entities;
using CRM.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CRM.Tests.Services;

public class ModuleServiceTests
{
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _moduleId = Guid.NewGuid();

    private ModuleService BuildService(ApplicationDbContext db) => new(
        new MSSQLRepository<Module, Guid>(db),
        new MSSQLRepository<TenantModule, Guid>(db),
        new MSSQLRepository<AuditLog, long>(db),
        db,
        new Mock<IMapper>().Object,
        new Mock<IHttpContextAccessor>().Object);

    private ApplicationDbContext SeedModule()
    {
        var db = TestDbContext.Create(_tenantId);
        db.Set<Module>().Add(new Module { Id = _moduleId, Name = "Inventory", Code = "INVENTORY", IsActive = true });
        db.SaveChanges();
        db.ChangeTracker.Clear();
        return db;
    }

    [Fact]
    public async Task ToggleAsync_enable_creates_active_tenant_module()
    {
        var db = SeedModule();
        var service = BuildService(db);

        var result = await service.ToggleAsync(new ToggleModuleRequest(_moduleId, true));

        result.IsSuccess.Should().BeTrue();
        db.ChangeTracker.Clear();
        var tm = db.Set<TenantModule>().IgnoreQueryFilters().Single();
        tm.ModuleId.Should().Be(_moduleId);
        tm.IsActive.Should().BeTrue();
        tm.ActivatedAt.Should().NotBeNull();
        tm.TenantId.Should().Be(_tenantId);
    }

    [Fact]
    public async Task ToggleAsync_disable_deactivates_existing_tenant_module()
    {
        var db = SeedModule();
        var service = BuildService(db);
        await service.ToggleAsync(new ToggleModuleRequest(_moduleId, true));
        db.ChangeTracker.Clear();

        var result = await service.ToggleAsync(new ToggleModuleRequest(_moduleId, false));

        result.IsSuccess.Should().BeTrue();
        db.ChangeTracker.Clear();
        var tm = db.Set<TenantModule>().IgnoreQueryFilters().Single();
        tm.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetTenantModulesAsync_returns_current_tenant_modules()
    {
        var db = SeedModule();
        var service = BuildService(db);
        await service.ToggleAsync(new ToggleModuleRequest(_moduleId, true));
        db.ChangeTracker.Clear();

        var result = await service.GetTenantModulesAsync();

        result.IsSuccess.Should().BeTrue();
        result.Content.Should().ContainSingle();
        result.Content[0].ModuleId.Should().Be(_moduleId);
        result.Content[0].ModuleCode.Should().Be("INVENTORY");
    }
}
