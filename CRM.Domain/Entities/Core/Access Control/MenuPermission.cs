using CRM.Base.Common.Domain.Entities;

namespace CRM.Domain.Entities;

public class MenuPermission : BaseEntity<Guid>  // Global
{
    public int MenuId { get; set; }
    public int PermissionId { get; set; }

    public Menu Menu { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}