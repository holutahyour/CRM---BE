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

    // Job title / position (e.g. "Operations Manager", "Agronomist"). Optional — added to
    // carry the Position column from the Role Allocation document when staff are imported.
    public string? Position { get; set; }

    // Reporting line (downliner -> manager). Self-referencing, optional. Added to carry the
    // Downliners relationships from the Role Allocation document when staff are imported.
    public Guid? ManagerId { get; set; }

    // Computed
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation
    [ForeignKey("DepartmentId")]
    public Department? Department { get; set; }

    [ForeignKey("ManagerId")]
    public User? Manager { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
}
