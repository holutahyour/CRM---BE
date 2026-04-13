using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("ordr_sales_order_items")]
public class SalesOrderItem : TenantEntity<Guid>
{
    public Guid SalesOrderId { get; set; }
    public Guid ItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal FulfilledQuantity { get; set; }
    public Guid? BatchId { get; set; }

    // Computed
    public decimal LineTotal => Quantity * UnitPrice;

    // Navigation
    public SalesOrder SalesOrder { get; set; } = null!;
    public Item Item { get; set; } = null!;
    public Batch? Batch { get; set; }
}