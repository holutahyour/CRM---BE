using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("invtry_item_locations")]
public class ItemLocation : TenantEntity<Guid>
{
    public int ItemId { get; set; }
    public int LocationId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Reserved { get; set; }
    public decimal Damaged { get; set; }

    // Computed
    public decimal Available => Quantity - Reserved - Damaged;

    // Navigation
    public Item Item { get; set; } = null!;
    public Location Location { get; set; } = null!;
}