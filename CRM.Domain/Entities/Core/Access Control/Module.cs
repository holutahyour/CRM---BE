using CRM.Base.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("sys_modules")]
public partial class Module : BaseEntity<Guid>  // Global
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string? Description { get; set; }
    public string? Version { get; set; }
    public Guid? CategoryId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ModuleCategory? Category { get; set; }
    public ICollection<TenantModule> TenantModules { get; set; } = [];
}