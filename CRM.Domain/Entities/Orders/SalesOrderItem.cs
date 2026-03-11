using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities.Orders;

[Table("ordr_sales_order_items")]
public class SalesOrderItem : TenantEntity<Guid>
{
    public int SalesOrderId { get; set; }
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal FulfilledQuantity { get; set; }
    public int? BatchId { get; set; }

    // Computed
    public decimal LineTotal => Quantity * UnitPrice;

    // Navigation
    public SalesOrder SalesOrder { get; set; } = null!;
    public Item Item { get; set; } = null!;
    public Batch? Batch { get; set; }
}