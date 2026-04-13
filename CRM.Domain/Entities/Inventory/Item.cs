using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("invtry_items")]
public class Item : TenantEntity<Guid>
{
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
    public decimal QuantityOnHand { get; set; } = 0; // Materialized exact quantity
    public decimal? CostPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public string? ImageUrl { get; set; }
    public string? StorageConditions { get; set; }
    public Guid? LocationId { get; set; }
    public string? ItemLocation { get; set; }

    // Navigation
    public Category? Category { get; set; }
    public Vendor? Vendor { get; set; }
    public Location? Location { get; set; }
    public ICollection<ItemLocation> ItemLocations { get; set; } = [];
    public ICollection<Batch> Batches { get; set; } = [];
    public ICollection<InventoryTransaction> Transactions { get; set; } = [];
}
