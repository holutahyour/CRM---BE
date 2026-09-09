using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

/// <summary>
/// One egg sale. Total and outstanding balance are derived by the UI from
/// quantity, price and paid rather than stored.
/// </summary>
[Table("sales_records")]
public class SalesRecord : TenantEntity<Guid>
{
    public DateOnly Date { get; set; }
    public string Customer { get; set; } = "";

    /// <summary>Crates sold.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Price per crate.</summary>
    public decimal Price { get; set; }

    public decimal Paid { get; set; }
    public string ModeOfPayment { get; set; } = "";
    public string? Remarks { get; set; }
}
