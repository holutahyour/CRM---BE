using CRM.Base.Domain.Entities;

namespace CRM.Domain.Entities;

public class UserRole : TenantEntity<Guid>
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
