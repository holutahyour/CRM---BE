using CRM.Base.Common.Domain.Entities;
using CRM.Data;
using CRM.Data.Seeds;
using CRM.Domain.Constants;
using CRM.Domain.Entities;
using CRM.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CRM.Tests.Services;

/// <summary>
/// The Processing reference-data seed runs on every dev start-up and every production --seed, against
/// a database that already holds the earlier inventory import. It must land everything under the
/// real SYSTEM tenant, reuse (not duplicate) the items the extractor matched, and be a no-op the
/// second time. Uses the real embedded JSON, so a missing EmbeddedResource fails here too.
/// </summary>
public class ProcessingDataSeederTests
{
    private static (SqliteTestDb Sqlite, ApplicationDbContext Db, Guid TenantId) NewSystemDb(bool withMainStore = true)
    {
        var sqlite = SqliteTestDb.Create();
        var db = sqlite.Context;
        var tenant = new Tenant { Name = "System", Code = "SYSTEM" };
        db.Tenants.Add(tenant);
        db.SaveChanges();

        if (withMainStore)
        {
            db.Locations.Add(new Location
            {
                Id = Guid.NewGuid(), Code = "MAINSTORE1", Name = "Main Store",
                Type = CRM.Domain.Enums.LocationType.Warehouse, TenantId = tenant.Id,
            });
        }
        var otherItems = new Category { Id = Guid.NewGuid(), Code = "OTHERITEMS", Name = "Other Items", TenantId = tenant.Id };
        db.Categories.Add(otherItems);
        db.Items.Add(new Item
        {
            Id = Guid.NewGuid(), Code = "OTHR-0241", Sku = "OTHR-0241", Name = "Beans", UnitType = "bags",
            CategoryId = otherItems.Id, QuantityOnHand = 25, TenantId = tenant.Id,
        });
        db.SaveChanges();
        return (sqlite, db, tenant.Id);
    }

    [Fact]
    public async Task Seeds_the_processing_locations_categories_materials_and_products()
    {
        var (sqlite, db, tenantId) = NewSystemDb();
        using var _ = sqlite;
        var materials = EupepsiaSeedData.ProcessingMaterials();
        var products = EupepsiaSeedData.ProcessingProducts();

        await new ProcessingDataSeeder(db).InitializeAsync();
        db.ChangeTracker.Clear();

        var locations = await db.Locations.IgnoreQueryFilters().Where(l => l.TenantId == tenantId).ToListAsync();
        locations.Select(l => l.Name).Should().BeEquivalentTo("Main Store", "Packaging Store");

        var categories = await db.Categories.IgnoreQueryFilters().Where(c => c.TenantId == tenantId).ToListAsync();
        categories.Select(c => c.Name).Should().Contain(ProcessingCategories.All);

        var processingItems = await db.Items.IgnoreQueryFilters()
            .Include(i => i.Category).Include(i => i.Location)
            .Where(i => i.TenantId == tenantId && i.Category != null && ProcessingCategories.All.Contains(i.Category.Name))
            .ToListAsync();
        processingItems.Should().HaveCount(materials.Count);

        // Every material is where the extractor put it.
        foreach (var m in materials)
        {
            var item = processingItems.Should().ContainSingle(i => i.Name == m.Name, m.Name).Subject;
            item.Category!.Name.Should().Be(m.CategoryName, m.Name);
            item.Location!.Name.Should().Be(m.LocationName, m.Name);
        }

        // Reused, not duplicated: Beans moved into Raw Produce with its stock intact.
        var beans = await db.Items.IgnoreQueryFilters().Include(i => i.Category)
            .Where(i => i.TenantId == tenantId && i.Name == "Beans").ToListAsync();
        beans.Should().ContainSingle();
        beans[0].Sku.Should().Be("OTHR-0241");
        beans[0].Category!.Name.Should().Be(ProcessingCategories.RawProduce);
        beans[0].QuantityOnHand.Should().Be(25);

        processingItems.Where(i => i.Sku != "OTHR-0241").Should().OnlyContain(i => i.QuantityOnHand == 0);

        var seededProducts = await db.ProcessingProducts.IgnoreQueryFilters().Where(p => p.TenantId == tenantId).ToListAsync();
        seededProducts.Should().HaveCount(products.Count);
        seededProducts.Single(p => p.Name == "Ginger").ProductCode.Should().Be("SFL/002/GIG2/A10/0525/01");
        seededProducts.Single(p => p.Name == "Pineapple").ProcessingDuration.Should().Be("12Hrs");
    }

    [Fact]
    public async Task A_second_run_creates_nothing()
    {
        var (sqlite, db, tenantId) = NewSystemDb();
        using var _ = sqlite;

        await new ProcessingDataSeeder(db).InitializeAsync();
        var counts = await CountsAsync(db, tenantId);
        await new ProcessingDataSeeder(db).InitializeAsync();

        (await CountsAsync(db, tenantId)).Should().Be(counts);
    }

    [Fact]
    public async Task Creates_the_main_store_when_the_inventory_import_has_not_run()
    {
        var (sqlite, db, tenantId) = NewSystemDb(withMainStore: false);
        using var _ = sqlite;

        await new ProcessingDataSeeder(db).InitializeAsync();

        (await db.Locations.IgnoreQueryFilters().AnyAsync(l => l.TenantId == tenantId && l.Name == "Main Store"))
            .Should().BeTrue();
    }

    [Fact]
    public async Task A_reuse_sku_that_is_not_in_the_database_is_created_instead()
    {
        var (sqlite, db, tenantId) = NewSystemDb();
        using var _ = sqlite;
        var reused = EupepsiaSeedData.ProcessingMaterials().First(m => m.ReuseSku != null && m.ReuseSku != "OTHR-0241");

        await new ProcessingDataSeeder(db).InitializeAsync();

        var item = await db.Items.IgnoreQueryFilters().SingleAsync(i => i.TenantId == tenantId && i.Name == reused.Name);
        item.Sku.Should().StartWith("PROC-");
    }

    [Fact]
    public async Task Generated_skus_are_sequential_and_unique_for_the_tenant()
    {
        var (sqlite, db, tenantId) = NewSystemDb();
        using var _ = sqlite;

        await new ProcessingDataSeeder(db).InitializeAsync();

        var skus = await db.Items.IgnoreQueryFilters().Where(i => i.TenantId == tenantId).Select(i => i.Sku).ToListAsync();
        skus.Should().OnlyHaveUniqueItems();
        skus.Should().Contain("PROC-0001");
        skus.Where(s => s.StartsWith("PROC-")).Should().OnlyContain(s => System.Text.RegularExpressions.Regex.IsMatch(s, @"^PROC-\d{4}$"));
    }

    [Fact]
    public async Task Everything_is_seeded_under_the_system_tenant_not_guid_empty()
    {
        var (sqlite, db, tenantId) = NewSystemDb();
        using var _ = sqlite;

        await new ProcessingDataSeeder(db).InitializeAsync();

        (await db.Items.IgnoreQueryFilters().AnyAsync(i => i.TenantId == Guid.Empty)).Should().BeFalse();
        (await db.ProcessingProducts.IgnoreQueryFilters().AnyAsync(p => p.TenantId == Guid.Empty)).Should().BeFalse();
        (await db.ProcessingProducts.IgnoreQueryFilters().CountAsync(p => p.TenantId == tenantId)).Should().BePositive();
    }

    private static async Task<(int Locations, int Categories, int Items, int Products)> CountsAsync(ApplicationDbContext db, Guid tenantId) => (
        await db.Locations.IgnoreQueryFilters().CountAsync(l => l.TenantId == tenantId),
        await db.Categories.IgnoreQueryFilters().CountAsync(c => c.TenantId == tenantId),
        await db.Items.IgnoreQueryFilters().CountAsync(i => i.TenantId == tenantId),
        await db.ProcessingProducts.IgnoreQueryFilters().CountAsync(p => p.TenantId == tenantId));
}
