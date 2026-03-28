using CRM.Base.Domain.Entities;
using CRM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

// Entity: Requisition
public class Requisition : TenantEntity<Guid>
{
    public string Title { get; set; } // e.g., "Marketing Campaign Materials"
    public Guid DepartmentId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Description { get; set; }
    public decimal Amount { get; set; } // e.g., 5000.00 (in Naira)
    public RequisitionStatus Status { get; set; } = RequisitionStatus.Pending;
    public string Reason { get; set; } // For rejected
    public Guid? SubmittedBy { get; set; } // User Id

    [ForeignKey("SubmittedBy")]
    public User SubmittedByUser { get; set; }

    public Guid? ActionedBy { get; set; } // User who approved or rejected

    [ForeignKey("ActionedBy")]
    public User ActionedByUser { get; set; }

    public Department Department { get; set; }
}
