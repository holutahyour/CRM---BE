using CRM.Base.Domain.Entities;

namespace CRM.Domain.Entities;

public class User : TenantEntity<Guid>
{
    public string EntraObjectId { get; set; } = "";
    public string Email { get; set; } = "";
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // Computed
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
