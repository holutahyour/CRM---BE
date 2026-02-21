using CRM.Base.Domain.Entities;

namespace CRM.Domain.Entities;

public class RolePermission : TenantEntity<Guid>
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
