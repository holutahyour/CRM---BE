using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("accl_roles")]
public partial class Role : TenantEntity<Guid>
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}