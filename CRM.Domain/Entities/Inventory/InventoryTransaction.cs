using CRM.Base.Domain.Entities;
using CRM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("invtry_inventory_transactions")]
public class InventoryTransaction : TenantEntity<Guid>
{
    public int ItemId { get; set; }
    public int? BatchId { get; set; }
    public int? LocationId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Quantity { get; set; }
    public int? ReferenceId { get; set; }
    public string? Notes { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    // Navigation
    public Item Item { get; set; } = null!;
    public Batch? Batch { get; set; }
    public Location? Location { get; set; }
}