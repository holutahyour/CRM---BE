using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("invtry_categories")]
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
