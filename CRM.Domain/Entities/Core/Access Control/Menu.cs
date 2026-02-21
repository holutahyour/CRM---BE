using CRM.Base.Common.Domain.Entities;

namespace CRM.Domain.Entities;

public class Menu : BaseEntity<Guid>  // Global — not tenant-scoped
{
    public string Name { get; set; } = "";
    public string Label { get; set; } = "";
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public int? ParentId { get; set; }
    public string? ModuleCode { get; set; }
    public int Position { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Menu? Parent { get; set; }
    public ICollection<Menu> Children { get; set; } = [];
    public ICollection<MenuPermission> MenuPermissions { get; set; } = [];
}