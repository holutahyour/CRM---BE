using CRM.Base.Domain.Entities;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

// Entity: Requisition
public class Requisition : TenantEntity<Guid>
{
    public string Title { get; set; } // e.g., "Marketing Campaign Materials"
    public Guid DepartmentId { get; set; }
    public DateTime Date { get; set; } // e.g., "2026-02-03"
    public string Description { get; set; }
    public decimal Amount { get; set; } // e.g., 5000.00 (in Naira)
    public RequisitionStatus Status { get; set; }
    public string Reason { get; set; } // For rejected
    public Guid? SubmittedBy { get; set; } // User Id

    public Department Department { get; set; }
}
