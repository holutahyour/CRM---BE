namespace CRM.Domain.DTOs;


public partial class CategoryResponse : CreateCategoryRequest
{
    public Guid Id { get; set; }

}

public class CreateCategoryRequest : UpdateCategoryRequest
{
    public string Code { get; set; } = string.Empty;

}

public class UpdateCategoryRequest
{
    public string Name { get; set; } = "";
    public Guid? ParentId { get; set; }
    public string? Description { get; set; }
}
