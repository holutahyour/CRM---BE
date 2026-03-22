using CRM.Base.Domain.Entities;
using CRM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("req_activities")]
public class Activity : TenantEntity<Guid>
{
    public ActivityType Type { get; set; } // e.g., "Requisition", "Incident"
    public string Description { get; set; }
    public string Status { get; set; } // e.g., "Pending", "Approved", "Open"
    public Guid? RelatedEntityId { get; set; } // e.g., Requisition Id
    public Guid? UserId { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime Timestamp { get; set; }

    // Navigation Property
    public Department Department { get; set; }
}
