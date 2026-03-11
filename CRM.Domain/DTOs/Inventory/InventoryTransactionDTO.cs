using CRM.Domain.Enums;

namespace CRM.Domain.DTOs;


public partial class InventoryTransactionResponse : CreateInventoryTransactionRequest
{
    public Guid Id { get; set; }

}

public class CreateInventoryTransactionRequest
{
    public string Code { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public int? BatchId { get; set; }
    public int? LocationId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Quantity { get; set; }
    public int? ReferenceId { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
}

public class UpdateInventoryTransactionRequest
{
    public string? Notes { get; set; }
}
