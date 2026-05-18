using CRM.Base.Domain.Entities;
using CRM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("accl_users")]
public class User : TenantEntity<Guid>
{
    public string EntraObjectId { get; set; } = "";
    public string Email { get; set; } = "";
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public int AccessLevel { get; set; } // e.g., Level 4
    public UserStatus Status { get; set; }
    public bool Onboarded { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public Guid? DepartmentId { get; set; }

    // Computed
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation
    [ForeignKey("DepartmentId")]
    public Department? Department { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
