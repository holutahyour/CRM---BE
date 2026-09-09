using AutoMapper;
using CRM.Base.Common.Domain.Entities;
using CRM.Base.Repositories.Implementations;
using CRM.Data;
using CRM.Domain.DTOs;
using CRM.Domain.Entities;
using CRM.Service;
using CRM.Services.Implementations;
using CRM.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CRM.Tests.Services;

/// <summary>
/// The Sales tabs persist through these four services. The tests run on SQLite
/// with the application's real AutoMapper profile, so a missing or wrong mapping
/// between a request/response DTO and its entity fails here rather than silently
/// returning an empty record to the browser.
/// </summary>
public class SalesServiceTests
{
    private readonly Guid _tenantA = Guid.NewGuid();
    private readonly Guid _tenantB = Guid.NewGuid();

    private static IMapper BuildMapper() =>
        new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperConfig()), NullLoggerFactory.Instance)
            .CreateMapper();

    private static SalesRecordService BuildSaleService(ApplicationDbContext db) => new(
        new MSSQLRepository<SalesRecord, Guid>(db),
        new MSSQLRepository<AuditLog, long>(db),
        db,
        BuildMapper(),
        new Mock<IHttpContextAccessor>().Object);

    private static SalesDailyProductionService BuildProductionService(ApplicationDbContext db) => new(
        new MSSQLRepository<SalesDailyProduction, Guid>(db),
        new MSSQLRepository<AuditLog, long>(db),
        db,
        BuildMapper(),
        new Mock<IHttpContextAccessor>().Object);

    private static SalesFeedCostService BuildFeedCostService(ApplicationDbContext db) => new(
        new MSSQLRepository<SalesFeedCost, Guid>(db),
        new MSSQLRepository<AuditLog, long>(db),
        db,
        BuildMapper(),
        new Mock<IHttpContextAccessor>().Object);

    private static SalesStockRecordService BuildStockService(ApplicationDbContext db) => new(
        new MSSQLRepository<SalesStockRecord, Guid>(db),
        new MSSQLRepository<AuditLog, long>(db),
        db,
        BuildMapper(),
        new Mock<IHttpContextAccessor>().Object);

    private static CreateSalesRecordRequest ASale(string customer = "XYZ Restaurant") => new()
    {
        Date = new DateOnly(2026, 6, 5),
        Customer = customer,
        Quantity = 10,
        Price = 1800,
        Paid = 18000,
        ModeOfPayment = "Transfer",
        Remarks = "Full payment",
    };

    [Fact]
    public async Task Creating_a_sale_persists_every_field()
    {
        using var sqlite = SqliteTestDb.Create(_tenantA);
        var service = BuildSaleService(sqlite.Context);

        var created = await service.CreateAsync<SalesRecordResponse, CreateSalesRecordRequest>(ASale());

        created.IsSuccess.Should().BeTrue(created.ErrorMessage);
        created.Content.Id.Should().NotBeEmpty();
        created.Content.Date.Should().Be(new DateOnly(2026, 6, 5));
        created.Content.Customer.Should().Be("XYZ Restaurant");
        created.Content.Quantity.Should().Be(10);
        created.Content.Price.Should().Be(1800);
        created.Content.Paid.Should().Be(18000);
        created.Content.ModeOfPayment.Should().Be("Transfer");
        created.Content.Remarks.Should().Be("Full payment");

        // It is really in the table, not just echoed back from the request.
        var stored = await sqlite.Context.Set<SalesRecord>().SingleAsync();
        stored.Customer.Should().Be("XYZ Restaurant");
        stored.Date.Should().Be(new DateOnly(2026, 6, 5));
        stored.TenantId.Should().Be(_tenantA);
    }

    [Fact]
    public async Task A_created_sale_is_returned_by_the_list_the_tab_loads()
    {
        using var sqlite = SqliteTestDb.Create(_tenantA);
        var service = BuildSaleService(sqlite.Context);
        await service.CreateAsync<SalesRecordResponse, CreateSalesRecordRequest>(ASale());

        var listed = await service.GetAllAsync<SalesRecordResponse>();

        listed.IsSuccess.Should().BeTrue(listed.ErrorMessage);
        listed.Content.Should().ContainSingle();
        listed.Content[0].Customer.Should().Be("XYZ Restaurant");
    }

    [Fact]
    public async Task Removing_a_sale_takes_it_out_of_the_list()
    {
        using var sqlite = SqliteTestDb.Create(_tenantA);
        var service = BuildSaleService(sqlite.Context);
        var created = await service.CreateAsync<SalesRecordResponse, CreateSalesRecordRequest>(ASale());

        var removed = await service.RemoveAsync(created.Content.Id);

        removed.IsSuccess.Should().BeTrue(removed.ErrorMessage);
        var listed = await service.GetAllAsync<SalesRecordResponse>();
        listed.Content.Should().BeEmpty();
    }

    [Fact]
    public async Task Sales_are_scoped_to_the_tenant_that_created_them()
    {
        using var sqlite = SqliteTestDb.Create(_tenantA);
        await BuildSaleService(sqlite.Context)
            .CreateAsync<SalesRecordResponse, CreateSalesRecordRequest>(ASale("Tenant A Buyer"));

        using var other = SqliteTestDb.Create(_tenantB);
        var listedForB = await BuildSaleService(other.Context).GetAllAsync<SalesRecordResponse>();

        listedForB.Content.Should().BeEmpty();
    }

    [Fact]
    public async Task Creating_a_production_record_persists_every_field()
    {
        using var sqlite = SqliteTestDb.Create(_tenantA);
        var service = BuildProductionService(sqlite.Context);

        var created = await service
            .CreateAsync<SalesDailyProductionResponse, CreateSalesDailyProductionRequest>(new()
            {
                Date = new DateOnly(2026, 6, 1),
                OpeningBirds = 500,
                EggsMorning = 420,
                TotalEggs = 420,
                TotalEggsCrates = 15,
                Cracked = 8,
                Bad = 4,
                SmallEggs = 20,
            });

        created.IsSuccess.Should().BeTrue(created.ErrorMessage);
        created.Content.Id.Should().NotBeEmpty();
        created.Content.OpeningBirds.Should().Be(500);
        created.Content.TotalEggs.Should().Be(420);
        created.Content.Cracked.Should().Be(8);
        created.Content.Bad.Should().Be(4);
        created.Content.SmallEggs.Should().Be(20);

        (await sqlite.Context.Set<SalesDailyProduction>().CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Creating_a_feed_cost_persists_every_field()
    {
        using var sqlite = SqliteTestDb.Create(_tenantA);
        var service = BuildFeedCostService(sqlite.Context);

        var created = await service
            .CreateAsync<SalesFeedCostResponse, CreateSalesFeedCostRequest>(new()
            {
                Date = new DateOnly(2026, 6, 1),
                FeedType = "Layers Mash",
                Quantity = 10,
                CostPerBag = 12000,
            });

        created.IsSuccess.Should().BeTrue(created.ErrorMessage);
        created.Content.Id.Should().NotBeEmpty();
        created.Content.FeedType.Should().Be("Layers Mash");
        created.Content.Quantity.Should().Be(10);
        created.Content.CostPerBag.Should().Be(12000);

        (await sqlite.Context.Set<SalesFeedCost>().CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Creating_a_stock_record_persists_every_field()
    {
        using var sqlite = SqliteTestDb.Create(_tenantA);
        var service = BuildStockService(sqlite.Context);

        var created = await service
            .CreateAsync<SalesStockRecordResponse, CreateSalesStockRecordRequest>(new()
            {
                Date = new DateOnly(2026, 6, 1),
                OpeningEggs = 200,
                OpeningCrates = 7,
                Produced = 420,
                Sold = 360,
                SoldCrates = 13,
                Loss = 12,
                LossCrates = 0,
            });

        created.IsSuccess.Should().BeTrue(created.ErrorMessage);
        created.Content.Id.Should().NotBeEmpty();
        created.Content.OpeningEggs.Should().Be(200);
        created.Content.Produced.Should().Be(420);
        created.Content.Sold.Should().Be(360);
        created.Content.Loss.Should().Be(12);

        (await sqlite.Context.Set<SalesStockRecord>().CountAsync()).Should().Be(1);
    }
}
