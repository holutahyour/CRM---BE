using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

/// <summary>
/// One Fresh Produce sale. Total and outstanding balance are derived by the UI
/// from quantity, price per kg and paid rather than stored.
/// </summary>
[Table("produce_sales")]
public class ProduceSale : TenantEntity<Guid>
{
    public DateOnly Date { get; set; }
    public string Customer { get; set; } = "";
    public string? Location { get; set; }
    public string? ProduceType { get; set; }

    /// <summary>"Grade A", "Grade B" or "Grade C".</summary>
    public string? Grade { get; set; }

    public string Category { get; set; } = "";

    /// <summary>Kilograms sold.</summary>
    public decimal Quantity { get; set; }

    public decimal PricePerKg { get; set; }
    public decimal Paid { get; set; }

    /// <summary>Shown as the sale's "Remarks" column.</summary>
    public string ModeOfPayment { get; set; } = "";

    public string PaymentStatus { get; set; } = "";
}
