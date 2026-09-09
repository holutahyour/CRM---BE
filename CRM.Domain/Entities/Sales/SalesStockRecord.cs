using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

/// <summary>
/// One day's egg stock movement. Closing stock is derived by the UI as
/// opening + produced - sold - loss rather than stored.
/// </summary>
[Table("sales_stock_records")]
public class SalesStockRecord : TenantEntity<Guid>
{
    public DateOnly Date { get; set; }
    public int OpeningEggs { get; set; }
    public int OpeningCrates { get; set; }
    public int Produced { get; set; }
    public int Sold { get; set; }
    public int SoldCrates { get; set; }
    public int Loss { get; set; }
    public int LossCrates { get; set; }
}
