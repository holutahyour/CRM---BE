using CRM.Base.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("sys_module_categories")]
public partial class ModuleCategory : BaseEntity<Guid>
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Module> Modules { get; set; } = [];
}