using CRM.Base.Domain.Entities;

namespace CRM.Domain.Entities;

public class UserRole : TenantEntity<Guid>
{
    public int UserId { get; set; }
    public int RoleId { get; set; }

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
