using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

/// <summary>
/// One feed purchase. Total cost is derived by the UI from quantity and cost
/// per bag rather than stored.
/// </summary>
[Table("sales_feed_costs")]
public class SalesFeedCost : TenantEntity<Guid>
{
    public DateOnly Date { get; set; }
    public string FeedType { get; set; } = "";

    /// <summary>Bags purchased.</summary>
    public decimal Quantity { get; set; }

    public decimal CostPerBag { get; set; }
}
