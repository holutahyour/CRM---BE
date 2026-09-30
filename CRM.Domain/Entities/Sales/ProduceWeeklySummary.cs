using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

/// <summary>
/// One week's Fresh Produce sales aggregate, entered by hand. Balance is
/// derived by the UI from total sales and total paid rather than stored.
/// </summary>
[Table("produce_weekly_summaries")]
public class ProduceWeeklySummary : TenantEntity<Guid>
{
    public DateOnly WeekStart { get; set; }
    public DateOnly WeekEnd { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalPaid { get; set; }
}
