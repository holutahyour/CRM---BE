using CRM.Domain.Entities;
using CRM.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CRM.Tests.Helpers;

/// <summary>
/// Smoke tests proving the in-memory SQLite test harness builds the real schema and behaves like a
/// relational store (FK navigation, unique indexes, soft-delete query filter).
/// </summary>
public class SqliteTestDbTests
{
    [Fact]
    public void EnsureCreated_builds_schema_and_seeds_static_reference_data()
    {
        using var sqlite = SqliteTestDb.Create();

        // The static HasData seed (Permissions) is applied by EnsureCreated against real SQL.
        sqlite.Context.Permissions.IgnoreQueryFilters().Count().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Can_persist_and_read_back_related_entities()
    {
        var tenantId = Guid.NewGuid();
        using var sqlite = SqliteTestDb.Create(tenantId);
        var db = sqlite.Context;

        var category = new Category { Id = Guid.NewGuid(), Name = "Electricals", TenantId = tenantId };
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Sku = "ELEC-0001",
            Name = "Meter Board",
            UnitType = "piece",
            CategoryId = category.Id,
            TenantId = tenantId,
        };
        db.Categories.Add(category);
        db.Items.Add(item);
        await db.SaveChangesAsync();

        db.ChangeTracker.Clear();

        var loaded = await db.Items.Include(i => i.Category).FirstAsync(i => i.Sku == "ELEC-0001");
        loaded.Name.Should().Be("Meter Board");
        loaded.Category!.Name.Should().Be("Electricals");
    }

    [Fact]
    public async Task Unique_sku_index_is_enforced_by_sqlite()
    {
        var tenantId = Guid.NewGuid();
        using var sqlite = SqliteTestDb.Create(tenantId);
        var db = sqlite.Context;

        db.Items.Add(new Item { Id = Guid.NewGuid(), Sku = "DUP-1", Name = "A", UnitType = "piece", TenantId = tenantId });
        await db.SaveChangesAsync();

        db.Items.Add(new Item { Id = Guid.NewGuid(), Sku = "DUP-1", Name = "B", UnitType = "piece", TenantId = tenantId });

        // The (TenantId, Sku) unique index only exists on a real relational engine — InMemory
        // would not throw here.
        var act = async () => await db.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Soft_delete_query_filter_hides_deleted_rows()
    {
        var tenantId = Guid.NewGuid();
        using var sqlite = SqliteTestDb.Create(tenantId);
        var db = sqlite.Context;

        var cat = new Category { Id = Guid.NewGuid(), Name = "Temp", TenantId = tenantId, IsDeleted = true };
        db.Categories.Add(cat);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        (await db.Categories.AnyAsync(c => c.Id == cat.Id)).Should().BeFalse();
        (await db.Categories.IgnoreQueryFilters().AnyAsync(c => c.Id == cat.Id)).Should().BeTrue();
    }
}
