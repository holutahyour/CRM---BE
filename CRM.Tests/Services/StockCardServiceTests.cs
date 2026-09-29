using CRM.Data;
using CRM.Domain.Constants;
using CRM.Domain.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Services.Implementations;
using CRM.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CRM.Tests.Services;

/// <summary>
/// Operations → Processing → Material Stock. The stock cards are views over Inventory, so every
/// balance here must reconcile to Item.QuantityOnHand — the figure the Inventory module shows.
/// </summary>
public class StockCardServiceTests
{
    private readonly Guid _tenant = Guid.NewGuid();

    private Category AddCategory(ApplicationDbContext db, string name)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Code = Guid.NewGuid().ToString("N")[..10],
            Name = name,
            TenantId = _tenant,
        };
        db.Categories.Add(category);
        return category;
    }

    private Item AddItem(ApplicationDbContext db, Category? category, string name, decimal onHand = 0)
    {
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Code = Guid.NewGuid().ToString("N")[..10],
            Sku = "SKU-" + Guid.NewGuid().ToString("N")[..8],
            Name = name,
            UnitType = "kg",
            CategoryId = category?.Id,
            QuantityOnHand = onHand,
            TenantId = _tenant,
        };
        db.Items.Add(item);
        return item;
    }

    private static StockMovementRequest Move(int day, decimal qty, string? where = null) => new()
    {
        Date = new DateOnly(2026, 6, day),
        Quantity = qty,
        WhereRequired = where,
    };

    [Fact]
    public async Task The_overview_lists_only_items_in_the_processing_categories()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var db = sqlite.Context;
        var produce = AddCategory(db, ProcessingCategories.RawProduce);
        var packaging = AddCategory(db, ProcessingCategories.Packaging);
        var electricals = AddCategory(db, "Electricals");
        AddItem(db, produce, "Hibiscus Flower");
        AddItem(db, packaging, "Polythene Pouch (8 x 12 inches)");
        AddItem(db, electricals, "Cable");
        AddItem(db, null, "Uncategorised");
        await db.SaveChangesAsync();

        var result = await new StockCardService(db).ListAsync();

        result.IsSuccess.Should().BeTrue(result.ErrorMessage);
        result.Content.Select(i => i.Name).Should()
            .BeEquivalentTo("Hibiscus Flower", "Polythene Pouch (8 x 12 inches)");
        result.Content.Single(i => i.Name == "Hibiscus Flower").CategoryName
            .Should().Be(ProcessingCategories.RawProduce);
    }

    [Fact]
    public async Task Receiving_then_issuing_builds_a_running_balance_and_moves_on_hand()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var db = sqlite.Context;
        var item = AddItem(db, AddCategory(db, ProcessingCategories.RawProduce), "Ginger");
        await db.SaveChangesAsync();
        var service = new StockCardService(db);

        (await service.ReceiveAsync(item.Id, Move(1, 10, "Received"))).IsSuccess.Should().BeTrue();
        var issued = await service.IssueAsync(item.Id, Move(2, 4, "Processing facility"));

        issued.IsSuccess.Should().BeTrue(issued.ErrorMessage);
        issued.Content.Rows.Should().HaveCount(2);

        var first = issued.Content.Rows[0];
        first.Date.Should().Be(new DateOnly(2026, 6, 1));
        (first.Opening, first.Received, first.Issued, first.Closing).Should().Be((0m, 10m, 0m, 10m));
        first.WhereRequired.Should().Be("Received");

        var second = issued.Content.Rows[1];
        second.Date.Should().Be(new DateOnly(2026, 6, 2));
        (second.Opening, second.Received, second.Issued, second.Closing).Should().Be((10m, 0m, 4m, 6m));
        second.WhereRequired.Should().Be("Processing facility");

        issued.Content.Item.QuantityOnHand.Should().Be(6);

        db.ChangeTracker.Clear();
        (await db.Items.IgnoreQueryFilters().SingleAsync(i => i.Id == item.Id)).QuantityOnHand.Should().Be(6);
        var txns = await db.InventoryTransactions.IgnoreQueryFilters()
            .Where(t => t.ItemId == item.Id).OrderBy(t => t.TransactionDate).ToListAsync();
        txns.Select(t => t.TransactionType).Should()
            .Equal(TransactionType.Purchase, TransactionType.Consumption);
        txns[0].TransactionDate.Date.Should().Be(new DateTime(2026, 6, 1));
    }

    [Fact]
    public async Task An_item_with_stock_from_before_the_stock_card_opens_at_that_stock()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var db = sqlite.Context;
        var item = AddItem(db, AddCategory(db, ProcessingCategories.RawProduce), "Hibiscus Flower", onHand: 50);
        await db.SaveChangesAsync();

        var result = await new StockCardService(db).ReceiveAsync(item.Id, Move(3, 10));

        result.IsSuccess.Should().BeTrue(result.ErrorMessage);
        var row = result.Content.Rows.Single();
        (row.Opening, row.Received, row.Closing).Should().Be((50m, 10m, 60m));
        result.Content.Item.QuantityOnHand.Should().Be(60);
    }

    [Fact]
    public async Task Issuing_more_than_is_on_hand_is_rejected_and_changes_nothing()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var db = sqlite.Context;
        var item = AddItem(db, AddCategory(db, ProcessingCategories.Packaging), "Tamper Proof Nylon", onHand: 6);
        await db.SaveChangesAsync();

        var result = await new StockCardService(db).IssueAsync(item.Id, Move(4, 7));

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("6");
        db.ChangeTracker.Clear();
        (await db.Items.IgnoreQueryFilters().SingleAsync(i => i.Id == item.Id)).QuantityOnHand.Should().Be(6);
        (await db.InventoryTransactions.IgnoreQueryFilters().CountAsync()).Should().Be(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task A_movement_must_have_a_positive_quantity(int quantity)
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var db = sqlite.Context;
        var item = AddItem(db, AddCategory(db, ProcessingCategories.RawProduce), "Onion", onHand: 20);
        await db.SaveChangesAsync();
        var service = new StockCardService(db);

        (await service.ReceiveAsync(item.Id, Move(5, quantity))).IsSuccess.Should().BeFalse();
        (await service.IssueAsync(item.Id, Move(5, quantity))).IsSuccess.Should().BeFalse();
        (await db.InventoryTransactions.IgnoreQueryFilters().CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task Items_outside_the_processing_categories_cannot_be_posted_to()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var db = sqlite.Context;
        var item = AddItem(db, AddCategory(db, "Electricals"), "Cable", onHand: 20);
        await db.SaveChangesAsync();
        var service = new StockCardService(db);

        (await service.ReceiveAsync(item.Id, Move(5, 1))).IsSuccess.Should().BeFalse();
        (await service.IssueAsync(item.Id, Move(5, 1))).IsSuccess.Should().BeFalse();
        (await service.GetAsync(item.Id)).IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task A_back_dated_entry_takes_its_place_by_date_and_the_card_still_ends_at_on_hand()
    {
        using var sqlite = SqliteTestDb.Create(_tenant);
        var db = sqlite.Context;
        var item = AddItem(db, AddCategory(db, ProcessingCategories.Ingredients), "Black Pepper");
        await db.SaveChangesAsync();
        var service = new StockCardService(db);

        await service.ReceiveAsync(item.Id, Move(10, 5));
        await service.ReceiveAsync(item.Id, Move(5, 3)); // entered late, dated earlier
        var result = await service.GetAsync(item.Id);

        result.IsSuccess.Should().BeTrue(result.ErrorMessage);
        result.Content.Rows.Select(r => r.Date.Day).Should().Equal(5, 10);
        result.Content.Rows.Select(r => r.Closing).Should().Equal(3m, 8m);
        result.Content.Rows[^1].Closing.Should().Be(result.Content.Item.QuantityOnHand);
    }
}
