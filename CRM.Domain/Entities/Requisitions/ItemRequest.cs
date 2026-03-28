using CRM.Base.Domain.Entities;
using CRM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

public class ItemRequest : TenantEntity<Guid>
{
    public Guid? ItemId { get; set; }
    public string ItemName { get; set; } = "";
    public decimal Quantity { get; set; }
    public string? Purpose { get; set; }
    public Guid DepartmentId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public ItemRequestStatus Status { get; set; } = ItemRequestStatus.Pending;
    public string? Reason { get; set; } // For rejected
    public Guid? SubmittedBy { get; set; } // User Id
    public Guid? ActionedBy { get; set; } // User who approved or rejected

    [ForeignKey("SubmittedBy")]
    public User SubmittedByUser { get; set; }


    [ForeignKey("ActionedBy")]
    public User ActionedByUser { get; set; }

    public Item? Item { get; set; }
    public Department Department { get; set; }

}
