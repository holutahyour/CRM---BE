using CRM.Base.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("accl_menu_permissions")]
public class MenuPermission : BaseEntity<Guid>  // Global
{
    public int MenuId { get; set; }
    public int PermissionId { get; set; }

    public Menu Menu { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}