using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("accl_user_roles")]
public partial class UserRole : TenantEntity<Guid>
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
