using CRM.Base.Common.Domain.Entities;

namespace CRM.Domain.Entities;

public class ModuleCategory : BaseEntity<Guid>
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Module> Modules { get; set; } = [];
}