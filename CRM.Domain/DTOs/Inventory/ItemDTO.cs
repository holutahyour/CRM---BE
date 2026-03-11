namespace CRM.Domain.DTOs;


public partial class ItemResponse : CreateItemRequest
{
    public Guid Id { get; set; }

}

public class CreateItemRequest
{
    public string Code { get; set; } = string.Empty;
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public Guid? CategoryId { get; set; }
    public Guid? VendorId { get; set; }
    public string? Description { get; set; }
    public string UnitType { get; set; } = "piece";
    public string? Barcode { get; set; }
    public bool BatchTracked { get; set; }
    public bool ExpiryTracked { get; set; }
    public decimal? MinStockLevel { get; set; }
    public decimal? ReorderQuantity { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public string? ImageUrl { get; set; }
    public string? StorageConditions { get; set; }
}

public class UpdateItemRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Barcode { get; set; }
    public decimal? MinStockLevel { get; set; }
    public decimal? ReorderQuantity { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public string? ImageUrl { get; set; }
    public string? StorageConditions { get; set; }
}
