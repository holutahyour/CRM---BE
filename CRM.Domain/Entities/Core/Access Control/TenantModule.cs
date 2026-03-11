using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("sys_tenant_modules")]
public partial class TenantModule : TenantEntity<Guid>
{
    public Guid ModuleId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ActivatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public Module Module { get; set; } = null!;
}