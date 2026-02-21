using CRM.Base.Domain.Entities;

namespace CRM.Domain.Entities.Inventory;

public class Category : TenantEntity<Guid>
{
    public string Name { get; set; } = "";
    public Guid? ParentId { get; set; }
    public string? Description { get; set; }

    // Navigation
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = [];
    public ICollection<Item> Items { get; set; } = [];
}
