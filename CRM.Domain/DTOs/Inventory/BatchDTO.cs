namespace CRM.Domain.DTOs;


public partial class BatchResponse : CreateBatchRequest
{
    public Guid Id { get; set; }

}

public class CreateBatchRequest
{
    public string Code { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string BatchNumber { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public decimal? InitialQuantity { get; set; }
    public decimal CurrentQuantity { get; set; }
    public string? SupplierBatchId { get; set; }
}

public class UpdateBatchRequest
{
    public string? SupplierBatchId { get; set; }
}
