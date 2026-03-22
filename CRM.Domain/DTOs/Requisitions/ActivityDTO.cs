using CRM.Domain.Enums;

namespace CRM.Domain.DTOs;

public partial class ActivityResponse : CreateActivityRequest
{
    public Guid Id { get; set; }

}

public partial class CreateActivityRequest
{
    public string Code { get; set; } = string.Empty;
    public ActivityType Type { get; set; } // e.g., "Requisition Approved", "Incident Reported"
    public string Description { get; set; }
    public string Status { get; set; } // e.g., "Pending", "Approved", "Open"
    public Guid? UserId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? RelatedEntityId { get; set; } // e.g., Requisition Id
    public DateTime Timestamp { get; set; }

}

public partial class UpdateActivityRequest : CreateActivityRequest
{
    public required long Id { get; set; }
}

