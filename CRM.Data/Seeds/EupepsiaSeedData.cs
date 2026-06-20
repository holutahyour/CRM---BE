using System.Reflection;
using System.Text.Json;

namespace CRM.Data.Seeds;

/// <summary>
/// Loads the Eupepsia / Soilless Farm Lab staging JSON (embedded resources) that was
/// produced from the two customer source documents by <c>tools/import/extract.py</c>.
///
/// IMPORTANT: many values in these files are CREATED/INFERRED (not present verbatim in the
/// source) — e.g. generated SKUs, inferred departments, inferred location types, parsed
/// quantities/units. Each staging record carries a <c>created</c> object describing exactly
/// which fields were generated; see <c>docs/IMPORT-NOTES.md</c>. The seeders below only
/// consume the concrete data fields, but the flags remain in the JSON for audit.
/// </summary>
public static class EupepsiaSeedData
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public static T Load<T>(string fileName)
    {
        var asm = Assembly.GetExecutingAssembly();
        var resourceName = asm.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("." + fileName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException(
                $"Embedded seed resource '{fileName}' not found. Available: {string.Join(", ", asm.GetManifestResourceNames())}");

        using var stream = asm.GetManifestResourceStream(resourceName)!;
        return JsonSerializer.Deserialize<T>(stream, JsonOpts)
            ?? throw new InvalidOperationException($"Seed resource '{fileName}' deserialized to null.");
    }

    public static List<DepartmentSeed> Departments() => Load<List<DepartmentSeed>>("departments.json");
    public static List<CategorySeed> Categories() => Load<List<CategorySeed>>("categories.json");
    public static List<LocationSeed> Locations() => Load<List<LocationSeed>>("locations.json");
    public static List<ItemSeed> Items() => Load<List<ItemSeed>>("items.json");
    public static List<TransactionSeed> Transactions() => Load<List<TransactionSeed>>("inventory-transactions.json");
}

// Staging DTOs — only the concrete fields the seeders need (the `created`/`source` audit
// fields in the JSON are intentionally ignored here).
public record DepartmentSeed(string Name, string Code, string Description, int StaffCount, decimal Budget, int ProjectsCount, decimal PercentOfTotal);
public record CategorySeed(string Name, string Description);
public record LocationSeed(string Name, string Type);
public record ItemSeed(string Sku, string Name, string CategoryName, string UnitType, decimal QuantityOnHand);
public record TransactionSeed(string ItemSku, string CategoryName, string LocationName, string TransactionType, decimal Quantity, string RawQuantity, string? TransactionDate, string Notes, string SourceSheet);
