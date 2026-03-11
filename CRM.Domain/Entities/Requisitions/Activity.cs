using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("req_activities")]
public class Activity : TenantEntity<Guid>
{
    public string Type { get; set; } // e.g., "Requisition Approved", "Incident Reported"
    public string Description { get; set; }
    public Guid? RelatedEntityId { get; set; } // e.g., Requisition Id
    public Guid? UserId { get; set; }
    public DateTime Timestamp { get; set; }
}
