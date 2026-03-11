using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("accl_role_permissions")]
public partial class RolePermission : TenantEntity<Guid>
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
