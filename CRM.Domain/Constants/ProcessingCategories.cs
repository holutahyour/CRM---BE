namespace CRM.Domain.Constants;

/// <summary>
/// The Inventory categories that hold the Batch Production Scheduling workbook's stock-card
/// materials. Operations → Processing → Material Stock shows, and posts to, only items in these.
/// </summary>
public static class ProcessingCategories
{
    public const string RawProduce = "Processing – Raw Produce";
    public const string Ingredients = "Processing – Ingredients";
    /// <summary>The "OTHER MATERIALS INVENTORY" sheet: packaging, plus PPE and small tools.</summary>
    public const string Packaging = "Processing – Packaging & Supplies";

    public static readonly IReadOnlyList<string> All = [RawProduce, Ingredients, Packaging];
}
