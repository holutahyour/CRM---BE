using AutoMapper;
using CRM.Base.Common.Domain.Entities;
using CRM.Base.Repositories.Implementations;
using CRM.Data;
using CRM.Domain.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
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
/// Operations → Processing record logs (order requests, batches, yield entries, products). Run on
/// SQLite with the real AutoMapper profile, so a missing DTO↔entity mapping fails here rather than
/// returning an empty record to the browser.
/// </summary>
public class OperationsServiceTests
{
    private readonly Guid _tenant = Guid.NewGuid();

    private static IMapper BuildMapper() =>
        new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperConfig()), NullLoggerFactory.Instance)
            .CreateMapper();

    private static OrderRequestService Orders(ApplicationDbContext db) => new(
        new MSSQLRepository<OrderRequest, Guid>(db), new MSSQLRepository<AuditLog, long>(db),
        db, BuildMapper(), new Mock<IHttpContextAccessor>().Object);

    private static ProductionBatchService Batches(ApplicationDbContext db) => new(
        new MSSQLRepository<ProductionBatch, Guid>(db), new MSSQLRepository<AuditLog, long>(db),
        db, BuildMapper(), new Mock<IHttpContextAccessor>().Object);

    private static YieldEntryService Yields(ApplicationDbContext db) => new(
        new MSSQLRepository<YieldEntry, Guid>(db), new MSSQLRepository<AuditLog, long>(db),
        db, BuildMapper(), new Mock<IHttpContextAccessor>().Object);

    private static ProcessingProductService Products(ApplicationDbContext db) => new(
        new MSSQLRepository<ProcessingProduct, Guid>(db), new MSSQLRepository<AuditLog, long>(db),
        db, BuildMapper(), new Mock<IHttpContextAccessor>().Object);

    private static CreateOrderRequestRequest AnOrder() => new()
    {
        RequestDate = new DateOnly(2026, 1, 16),
        CustomerCode = "CUS-00529",
        CustomerName = "Staples and Veggies",
        Products = "Habanero pepper",
        ActivitiesRequired = "Dehydration and grinding",
        VolumeRequired = "1346.617kg",
        DeliveryDate = new DateOnly(2026, 1, 22),
        DeliveryLocation = "Abuja",
        Status = ProcessingStatus.OnHold,
    };

    private static CreateProductionBatchRequest ABatch() => new()
    {
        CustomerCode = "CUS-00359",
        CustomerName = "Odun African Fine Foods",
        BatchCode = "HI925001",
        ProductNames = "Dehydrated hibiscus cut flowers",
        ProductCode = "SFL/001/HIG3/0925/001",
        Quantity = 5000,
        QuantityUnit = "kg",
        StartDate = new DateOnly(2025, 10, 9),
        EndDate = new DateOnly(2025, 11, 9),
        LeadTime = "11 hours",
        WorkCenters = "Packaging Area & Sterilization Area",
        Operators = "Kareem & Sales Team",
        Status = ProcessingStatus.Complete,
        OnTimeDeliveryPercent = 80,
    };

    private async Task<Item> AddProduce(ApplicationDbContext db, string name = "Red Habanero")
    {
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Code = Guid.NewGuid().ToString("N")[..10],
            Sku = "SKU-" + Guid.NewGuid().ToString("N")[..8],
            Name = name,
            UnitType = "kg",
            TenantId = _tenant,
        };
        db.Items.Add(item);
        await db.SaveChangesAsync();
        return item;
    }

    // ── Order requests ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Creating_an_order_request_persists_every_field_for_the_tenant()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var service = Orders(sqlite.Context);

        var created = await service.CreateAsync<OrderRequestResponse, CreateOrderRequestRequest>(AnOrder());

        created.IsSuccess.Should().BeTrue(created.ErrorMessage);
        created.Content.Id.Should().NotBeEmpty();
        created.Content.CustomerCode.Should().Be("CUS-00529");
        created.Content.CustomerName.Should().Be("Staples and Veggies");
        created.Content.VolumeRequired.Should().Be("1346.617kg");
        created.Content.DeliveryDate.Should().Be(new DateOnly(2026, 1, 22));
        created.Content.Status.Should().Be(ProcessingStatus.OnHold);

        var stored = await sqlite.Context.OrderRequests.IgnoreQueryFilters().SingleAsync();
        stored.TenantId.Should().Be(_tenant);
        (await service.GetAllAsync<OrderRequestResponse>()).Content.Should().ContainSingle();
    }

    [Fact]
    public async Task An_order_request_without_a_customer_is_rejected()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var request = AnOrder();
        request.CustomerName = "  ";

        var created = await Orders(sqlite.Context)
            .CreateAsync<OrderRequestResponse, CreateOrderRequestRequest>(request);

        created.IsSuccess.Should().BeFalse();
        created.Message.Should().Contain("Customer");
        (await sqlite.Context.OrderRequests.IgnoreQueryFilters().CountAsync()).Should().Be(0);
    }

    // ── Production batches ──────────────────────────────────────────────────────

    [Fact]
    public async Task Creating_a_batch_persists_every_field()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);

        var created = await Batches(sqlite.Context)
            .CreateAsync<ProductionBatchResponse, CreateProductionBatchRequest>(ABatch());

        created.IsSuccess.Should().BeTrue(created.ErrorMessage);
        created.Content.BatchCode.Should().Be("HI925001");
        created.Content.ProductCode.Should().Be("SFL/001/HIG3/0925/001");
        created.Content.Quantity.Should().Be(5000);
        created.Content.WorkCenters.Should().Be("Packaging Area & Sterilization Area");
        created.Content.OnTimeDeliveryPercent.Should().Be(80);
    }

    [Fact]
    public async Task A_batch_that_ends_before_it_starts_is_rejected()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var request = ABatch();
        request.EndDate = new DateOnly(2025, 10, 1);

        var created = await Batches(sqlite.Context)
            .CreateAsync<ProductionBatchResponse, CreateProductionBatchRequest>(request);

        created.IsSuccess.Should().BeFalse();
        created.Message.Should().Contain("End date");
        (await sqlite.Context.ProductionBatches.IgnoreQueryFilters().CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task A_batch_with_on_time_delivery_over_100_percent_is_rejected()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var request = ABatch();
        request.OnTimeDeliveryPercent = 120;

        var created = await Batches(sqlite.Context)
            .CreateAsync<ProductionBatchResponse, CreateProductionBatchRequest>(request);

        created.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task A_batch_cannot_link_to_an_order_request_it_cannot_see()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var db = sqlite.Context;
        var foreign = new OrderRequest
        {
            Id = Guid.NewGuid(),
            Code = "FOREIGN001",
            RequestDate = new DateOnly(2026, 1, 1),
            CustomerName = "Another tenant's customer",
            Products = "Ginger",
            TenantId = Guid.NewGuid(),
        };
        db.OrderRequests.Add(foreign);
        await db.SaveChangesAsync();

        var missing = ABatch();
        missing.OrderRequestId = Guid.NewGuid();
        var crossTenant = ABatch();
        crossTenant.OrderRequestId = foreign.Id;

        (await Batches(db).CreateAsync<ProductionBatchResponse, CreateProductionBatchRequest>(missing))
            .IsSuccess.Should().BeFalse();
        (await Batches(db).CreateAsync<ProductionBatchResponse, CreateProductionBatchRequest>(crossTenant))
            .IsSuccess.Should().BeFalse();
        (await db.ProductionBatches.IgnoreQueryFilters().CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task A_batch_can_link_to_its_order_request()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var order = await Orders(sqlite.Context)
            .CreateAsync<OrderRequestResponse, CreateOrderRequestRequest>(AnOrder());
        var request = ABatch();
        request.OrderRequestId = order.Content.Id;

        var created = await Batches(sqlite.Context)
            .CreateAsync<ProductionBatchResponse, CreateProductionBatchRequest>(request);

        created.IsSuccess.Should().BeTrue(created.ErrorMessage);
        created.Content.OrderRequestId.Should().Be(order.Content.Id);
    }

    [Fact]
    public async Task Updating_a_batch_persists_the_change_and_is_validated()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var service = Batches(sqlite.Context);
        var created = await service.CreateAsync<ProductionBatchResponse, CreateProductionBatchRequest>(ABatch());

        var update = ABatch();
        update.Status = ProcessingStatus.NeedsReview;
        update.QualityChecks = "Q&A";
        var updated = await service.UpdateAsync(created.Content.Id, update);

        var invalid = ABatch();
        invalid.EndDate = new DateOnly(2020, 1, 1);
        var rejected = await service.UpdateAsync(created.Content.Id, invalid);

        updated.IsSuccess.Should().BeTrue(updated.ErrorMessage);
        rejected.IsSuccess.Should().BeFalse();
        sqlite.Context.ChangeTracker.Clear();
        var stored = await sqlite.Context.ProductionBatches.IgnoreQueryFilters().SingleAsync();
        stored.Status.Should().Be(ProcessingStatus.NeedsReview);
        stored.QualityChecks.Should().Be("Q&A");
        stored.EndDate.Should().Be(new DateOnly(2025, 11, 9));
    }

    // ── Yield entries ───────────────────────────────────────────────────────────

    [Fact]
    public void Yield_is_the_last_recorded_stage_over_the_weighed_input()
    {
        new YieldEntryResponse { InputQuantity = 100, InputUnit = "kg", DehydratedWeightKg = 10, GrindWeightKg = 9 }
            .YieldPercent.Should().Be(9);

        var hibiscus = new YieldEntryResponse
        {
            InputQuantity = 30, InputUnit = "bags", InputWeightKg = 743.9m,
            GrindWeightKg = 733.75m, WasteKg = 10.15m,
        };
        hibiscus.InputKg.Should().Be(743.9m);
        hibiscus.OutputKg.Should().Be(733.75m);
        hibiscus.WastePercent.Should().BeApproximately(1.3644m, 0.0001m);
    }

    [Fact]
    public void Output_falls_back_through_the_stages_that_were_recorded()
    {
        new YieldEntryResponse { InputQuantity = 186.76m, InputUnit = "kg", DehydratedWeightKg = 15.7m }
            .OutputKg.Should().Be(15.7m);
        new YieldEntryResponse { InputQuantity = 105, InputUnit = "kg", CutWeightKg = 103 }
            .OutputKg.Should().Be(103);
        new YieldEntryResponse { InputQuantity = 25, InputUnit = "kg", GrindWeightKg = 18.85m, SecondGrindWeightKg = 18.71m }
            .OutputKg.Should().Be(18.71m);
    }

    [Fact]
    public void Yield_is_unknown_when_the_input_was_not_weighed_or_is_zero()
    {
        var counted = new YieldEntryResponse { InputQuantity = 5, InputUnit = "bags", DehydratedWeightKg = 12 };
        counted.InputKg.Should().BeNull();
        counted.YieldPercent.Should().BeNull();

        var zero = new YieldEntryResponse { InputQuantity = 0, InputUnit = "kg", DehydratedWeightKg = 0, WasteKg = 0 };
        zero.YieldPercent.Should().BeNull();
        zero.WastePercent.Should().BeNull();
    }

    [Fact]
    public async Task Creating_a_yield_entry_returns_its_computed_yield()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var produce = await AddProduce(sqlite.Context);

        var created = await Yields(sqlite.Context).CreateAsync<YieldEntryResponse, CreateYieldEntryRequest>(new()
        {
            Date = new DateOnly(2026, 1, 16),
            ProduceItemId = produce.Id,
            InputQuantity = 1346.62m,
            InputUnit = "kg",
            DehydratedWeightKg = 121,
            GrindWeightKg = 115.71m,
        });

        created.IsSuccess.Should().BeTrue(created.ErrorMessage);
        created.Content.ProduceItemId.Should().Be(produce.Id);
        created.Content.GrindWeightKg.Should().Be(115.71m);
        created.Content.YieldPercent.Should().BeApproximately(8.5926m, 0.0001m);
    }

    [Fact]
    public async Task A_yield_entry_with_a_negative_weight_is_rejected()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var produce = await AddProduce(sqlite.Context);

        var created = await Yields(sqlite.Context).CreateAsync<YieldEntryResponse, CreateYieldEntryRequest>(new()
        {
            Date = new DateOnly(2026, 1, 16),
            ProduceItemId = produce.Id,
            InputQuantity = 10,
            InputUnit = "kg",
            WasteKg = -1,
        });

        created.IsSuccess.Should().BeFalse();
        (await sqlite.Context.YieldEntries.IgnoreQueryFilters().CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task A_yield_entry_for_an_unknown_produce_is_rejected()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);

        var created = await Yields(sqlite.Context).CreateAsync<YieldEntryResponse, CreateYieldEntryRequest>(new()
        {
            Date = new DateOnly(2026, 1, 16),
            ProduceItemId = Guid.NewGuid(),
            InputQuantity = 10,
            InputUnit = "kg",
        });

        created.IsSuccess.Should().BeFalse();
        created.Message.Should().Contain("Produce");
    }

    // ── Products ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task A_second_product_with_the_same_name_is_rejected()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var service = Products(sqlite.Context);

        var first = await service.CreateAsync<ProcessingProductResponse, CreateProcessingProductRequest>(
            new() { Name = "Ginger", ProductCode = "SFL/002/GIG2/A10/0525/01", Sku = "GIG2" });
        var duplicate = await service.CreateAsync<ProcessingProductResponse, CreateProcessingProductRequest>(
            new() { Name = " ginger " });

        first.IsSuccess.Should().BeTrue(first.ErrorMessage);
        first.Content.ProductCode.Should().Be("SFL/002/GIG2/A10/0525/01");
        duplicate.IsSuccess.Should().BeFalse();
        duplicate.Message.Should().Contain("already exists");
        (await sqlite.Context.ProcessingProducts.IgnoreQueryFilters().CountAsync()).Should().Be(1);
    }
}
