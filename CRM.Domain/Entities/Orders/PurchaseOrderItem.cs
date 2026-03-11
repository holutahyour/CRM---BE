using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities.Orders;

[Table("ordr_purchase_order_items")]
public class PurchaseOrderItem : TenantEntity<Guid>
{
    public int PurchaseOrderId { get; set; }
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public int? LocationId { get; set; }
    public string? BatchNumber { get; set; }

    // Computed
    public decimal LineTotal => Quantity * UnitPrice;

    // Navigation
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Item Item { get; set; } = null!;
    public Location? Location { get; set; }
}
