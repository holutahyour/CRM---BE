using CRM.Base.Domain.Entities;
using CRM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities.Orders;

[Table("ordr_sales_orders")]
public class SalesOrder : TenantEntity<Guid>
{
    public string OrderNumber { get; set; } = "";
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public DateTime OrderDate { get; set; }
    public SalesOrderStatus Status { get; set; } = SalesOrderStatus.Draft;
    public decimal TotalAmount { get; set; }
    public string? EcomOrderId { get; set; }
    public Guid? DeliveryLocationId { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public ICollection<SalesOrderItem> Items { get; set; } = [];
}
