using CRM.Base.Common;
using CRM.Domain.Enums;

namespace CRM.Data.Seeds;

/// <summary>
/// Seeds the Eupepsia / Soilless Farm Lab inventory from the "SHIPMENTS RECEIVED CARD" workbook:
/// the de-duplicated master item catalogue (Categories, Locations, Items) plus the full receiving
/// history (one InventoryTransaction per source row). Data comes from the embedded staging JSON
/// (see <see cref="EupepsiaSeedData"/>) produced by <c>tools/import/extract.py</c>.
///
/// CREATED/INFERRED values (flagged in the JSON and docs/IMPORT-NOTES.md): generated SKUs,
/// the "Project Supplies" catch-all category, the default "Main Store" location and all inferred
/// location Types, parsed UnitType, and QuantityOnHand summed from parsed receipt quantities.
/// Every step is guarded with an existence check so re-running is idempotent.
/// </summary>
public class InventoryDataSeeder
{
    private readonly ApplicationDbContext _context;

    public InventoryDataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> InitializeAsync()
    {
        var tenant = await _context.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Code == "SYSTEM");
        var tenantId = tenant?.Id ?? Guid.Empty;

        var categoryByName = await SeedCategoriesAsync(tenantId);
        var locationByName = await SeedLocationsAsync(tenantId);
        var itemBySku = await SeedItemsAsync(tenantId, categoryByName);
        var txnCount = await SeedTransactionsAsync(tenantId, itemBySku, locationByName);

        return $"Inventory seeded: {categoryByName.Count} categories, {locationByName.Count} locations, "
             + $"{itemBySku.Count} items, {txnCount} transactions.";
    }

    private async Task<Dictionary<string, Guid>> SeedCategoriesAsync(Guid tenantId)
    {
        if (!await _context.Categories.IgnoreQueryFilters().AnyAsync(c => c.TenantId == tenantId))
        {
            var rows = EupepsiaSeedData.Categories().Select(c => new Category
            {
                Id = Guid.NewGuid(),
                Code = RandomGenerator.RandomString(10),
                Name = c.Name,
                Description = c.Description,
                TenantId = tenantId,
                CreatedBy = "SYSTEM",
                CreatedOn = DateTime.UtcNow,
                LastModifiedBy = "SYSTEM",
                LastModifiedOn = DateTime.UtcNow,
            }).ToList();
            await _context.Categories.AddRangeAsync(rows);
            await _context.SaveChangesAsync();
        }

        return await _context.Categories.IgnoreQueryFilters()
            .Where(c => c.TenantId == tenantId)
            .ToDictionaryAsync(c => c.Name, c => c.Id);
    }

    private async Task<Dictionary<string, Guid>> SeedLocationsAsync(Guid tenantId)
    {
        if (!await _context.Locations.IgnoreQueryFilters().AnyAsync(l => l.TenantId == tenantId))
        {
            var rows = EupepsiaSeedData.Locations().Select(l => new Location
            {
                Id = Guid.NewGuid(),
                Code = RandomGenerator.RandomString(10),
                Name = l.Name,
                Type = Enum.TryParse<LocationType>(l.Type, true, out var t) ? t : LocationType.Warehouse,
                TenantId = tenantId,
                CreatedBy = "SYSTEM",
                CreatedOn = DateTime.UtcNow,
                LastModifiedBy = "SYSTEM",
                LastModifiedOn = DateTime.UtcNow,
            }).ToList();
            await _context.Locations.AddRangeAsync(rows);
            await _context.SaveChangesAsync();
        }

        return await _context.Locations.IgnoreQueryFilters()
            .Where(l => l.TenantId == tenantId)
            .ToDictionaryAsync(l => l.Name, l => l.Id);
    }

    private async Task<Dictionary<string, Guid>> SeedItemsAsync(Guid tenantId, Dictionary<string, Guid> categoryByName)
    {
        if (!await _context.Items.IgnoreQueryFilters().AnyAsync(i => i.TenantId == tenantId))
        {
            var rows = EupepsiaSeedData.Items().Select(i => new Item
            {
                Id = Guid.NewGuid(),
                Code = i.Sku,
                Sku = i.Sku,
                Name = i.Name,
                CategoryId = categoryByName.TryGetValue(i.CategoryName, out var cid) ? cid : (Guid?)null,
                UnitType = string.IsNullOrWhiteSpace(i.UnitType) ? "piece" : i.UnitType,
                QuantityOnHand = i.QuantityOnHand,
                TenantId = tenantId,
                CreatedBy = "SYSTEM",
                CreatedOn = DateTime.UtcNow,
                LastModifiedBy = "SYSTEM",
                LastModifiedOn = DateTime.UtcNow,
            }).ToList();

            // Insert in batches to keep the remote round-trips and change-tracker manageable.
            const int batchSize = 500;
            for (int i = 0; i < rows.Count; i += batchSize)
            {
                await _context.Items.AddRangeAsync(rows.Skip(i).Take(batchSize));
                await _context.SaveChangesAsync();
            }
        }

        return await _context.Items.IgnoreQueryFilters()
            .Where(i => i.TenantId == tenantId)
            .ToDictionaryAsync(i => i.Sku, i => i.Id);
    }

    private async Task<int> SeedTransactionsAsync(Guid tenantId, Dictionary<string, Guid> itemBySku, Dictionary<string, Guid> locationByName)
    {
        var seeds = EupepsiaSeedData.Transactions();

        var existing = await _context.InventoryTransactions.IgnoreQueryFilters().CountAsync(t => t.TenantId == tenantId);
        if (existing == seeds.Count)
            return existing; // already fully seeded
        if (existing > 0)
        {
            // Partial load (e.g. a previous run dropped its connection mid-way). Clear and redo
            // so the receiving history is complete and consistent. Hard delete (no soft-delete).
            await _context.InventoryTransactions.IgnoreQueryFilters()
                .Where(t => t.TenantId == tenantId).ExecuteDeleteAsync();
        }

        var rows = new List<InventoryTransaction>(seeds.Count);
        foreach (var s in seeds)
        {
            if (!itemBySku.TryGetValue(s.ItemSku, out var itemId))
                continue; // item not found (should not happen) — skip rather than fabricate

            rows.Add(new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                Code = RandomGenerator.RandomString(10),
                ItemId = itemId,
                LocationId = locationByName.TryGetValue(s.LocationName, out var lid) ? lid : (Guid?)null,
                TransactionType = Enum.TryParse<TransactionType>(s.TransactionType, true, out var tt) ? tt : TransactionType.Purchase,
                Quantity = s.Quantity,
                Notes = string.IsNullOrWhiteSpace(s.Notes) ? null : s.Notes,
                TransactionDate = DateTime.TryParse(s.TransactionDate, out var d) ? d : DateTime.UtcNow,
                TenantId = tenantId,
                CreatedBy = "SYSTEM",
                CreatedOn = DateTime.UtcNow,
                LastModifiedBy = "SYSTEM",
                LastModifiedOn = DateTime.UtcNow,
            });
        }

        const int batchSize = 200;
        for (int i = 0; i < rows.Count; i += batchSize)
        {
            await _context.InventoryTransactions.AddRangeAsync(rows.Skip(i).Take(batchSize));
            await _context.SaveChangesAsync();
        }
        return rows.Count;
    }
}
