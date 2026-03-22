using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("invtry_batches")]
public class Batch : TenantEntity<Guid>
{
    public Guid ItemId { get; set; }
    public string BatchNumber { get; set; } = "";
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public decimal? InitialQuantity { get; set; }
    public decimal CurrentQuantity { get; set; }
    public string? SupplierBatchId { get; set; }

    // Navigation
    public Item Item { get; set; } = null!;
}