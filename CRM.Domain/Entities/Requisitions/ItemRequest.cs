using CRM.Base.Domain.Entities;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class ItemRequest : TenantEntity<Guid>
{
    public Guid? ItemId { get; set; }
    public string ItemName { get; set; } = "";
    public decimal Quantity { get; set; }
    public string? Purpose { get; set; }
    public Guid DepartmentId { get; set; }
    public DateTime Date { get; set; } // e.g., "2026-01-30"
    public ItemRequestStatus Status { get; set; }
    public string? Reason { get; set; } // For rejected
    public Guid? SubmittedBy { get; set; } // User Id

    public Item? Item { get; set; }
    public Department Department { get; set; }

}
