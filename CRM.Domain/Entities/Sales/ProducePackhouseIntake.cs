using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

/// <summary>
/// One Fresh Produce packhouse intake: the day's harvest of one produce type,
/// graded A/B/C with the rest rejected. Accepted weight (A + B + C) is derived
/// by the UI rather than stored.
/// </summary>
[Table("produce_packhouse_intake")]
public class ProducePackhouseIntake : TenantEntity<Guid>
{
    public DateOnly Date { get; set; }
    public string ProduceType { get; set; } = "";

    /// <summary>Kilograms graded A.</summary>
    public decimal GradeA { get; set; }

    /// <summary>Kilograms graded B.</summary>
    public decimal GradeB { get; set; }

    /// <summary>Kilograms graded C.</summary>
    public decimal GradeC { get; set; }

    /// <summary>Kilograms rejected at intake — reported as spoilage.</summary>
    public decimal Rejected { get; set; }

    /// <summary>Kilograms harvested before grading.</summary>
    public decimal QuantityHarvested { get; set; }

    public string? Remarks { get; set; }
}
