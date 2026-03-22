using CRM.Base.Domain.Entities;
using CRM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities.Orders;

[Table("ordr_purchase_orders")]
public class PurchaseOrder : TenantEntity<Guid>
{
    public string OrderNumber { get; set; } = "";
    public Guid VendorId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string? QualityNotes { get; set; }

    // Navigation
    public Vendor Vendor { get; set; } = null!;
    public ICollection<PurchaseOrderItem> Items { get; set; } = [];
}
