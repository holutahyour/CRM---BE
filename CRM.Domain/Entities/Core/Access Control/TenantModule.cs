using CRM.Base.Domain.Entities;

namespace CRM.Domain.Entities;

public class TenantModule : TenantEntity<Guid>
{
    public int ModuleId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ActivatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public Module Module { get; set; } = null!;
}