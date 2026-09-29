using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

/// <summary>
/// One processing run of one produce — a row of the "PROCESSING ACTIVITIES" sheet, where each
/// produce had its own block of columns. Every stage weight is optional because each produce skips
/// different stages (hibiscus is weighed and ground twice; leaves are cut and dehydrated).
///
/// Yield and waste percentages are deliberately absent: the response derives them from these
/// weights, so storing them would let a row disagree with its own figures.
/// </summary>
[Table("ops_yield_entries")]
public class YieldEntry : TenantEntity<Guid>
{
    public DateOnly Date { get; set; }

    /// <summary>The produce, as an Inventory item in the "Processing – Raw Produce" category.</summary>
    public Guid ProduceItemId { get; set; }

    public Guid? ProductionBatchId { get; set; }

    /// <summary>"Day" / "Night", as the hibiscus rows record it.</summary>
    public string? Shift { get; set; }

    public decimal? InputQuantity { get; set; }

    /// <summary>"kg", "bags", "crates"…</summary>
    public string? InputUnit { get; set; }

    /// <summary>Weighed input when the quantity was counted in bags or crates.</summary>
    public decimal? InputWeightKg { get; set; }

    public decimal? CutWeightKg { get; set; }
    public decimal? DehydratedWeightKg { get; set; }
    public decimal? GrindWeightKg { get; set; }
    public decimal? SecondGrindWeightKg { get; set; }
    public decimal? WasteKg { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Item ProduceItem { get; set; } = null!;
    public ProductionBatch? ProductionBatch { get; set; }
}
