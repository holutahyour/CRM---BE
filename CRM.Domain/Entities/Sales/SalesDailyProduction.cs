using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

/// <summary>
/// One day's egg production for the Sales department (EPL Poultry).
///
/// Total loss and good eggs are deliberately absent: the UI derives both from
/// these columns, so storing them would let a row disagree with its own totals.
/// </summary>
[Table("sales_daily_production")]
public class SalesDailyProduction : TenantEntity<Guid>
{
    public DateOnly Date { get; set; }
    public int OpeningBirds { get; set; }
    public int EggsMorning { get; set; }
    public int TotalEggs { get; set; }
    public int TotalEggsCrates { get; set; }
    public int Cracked { get; set; }
    public int Bad { get; set; }
    public int SmallEggs { get; set; }
}
