using CRM.Base.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("accl_permissions")]
public partial class Permission : BaseEntity<Guid>  // Global — not tenant-scoped
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string? ModuleCode { get; set; }
    public string? Description { get; set; }

    // Navigation
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public ICollection<MenuPermission> MenuPermissions { get; set; } = [];
}
