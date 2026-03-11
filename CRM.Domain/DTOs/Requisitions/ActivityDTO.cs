namespace CRM.Domain.DTOs;

public partial class ActivityResponse : CreateActivityRequest
{
    public Guid Id { get; set; }

}

public partial class CreateActivityRequest
{
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } // e.g., "Requisition Approved", "Incident Reported"
    public string Description { get; set; }
    public Guid? RelatedEntityId { get; set; } // e.g., Requisition Id
    public Guid? UserId { get; set; }
    public DateTime Timestamp { get; set; }

}

public partial class UpdateActivityRequest
{
    public required long Id { get; set; }
    public required string Name { get; set; }

    public string Population { get; set; } = string.Empty;

    public string Area { get; set; } = string.Empty;
}

