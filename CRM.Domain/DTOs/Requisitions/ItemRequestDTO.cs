using CRM.Domain.Enums;

namespace CRM.Domain.DTOs;

public partial class ItemRequestResponse : CreateItemRequestRequest
{
    public Guid Id { get; set; }
    public ItemRequestStatus Status { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Reason { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public string ActionedByName { get; set; } = string.Empty;
}

public partial class CreateItemRequestRequest
{
    public string Code { get; set; } = string.Empty;
    public Guid? ItemId { get; set; }
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public string Purpose { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid? SubmittedBy { get; set; }
}

public class UpdateItemRequestRequest
{
    public int? Quantity { get; set; }
    public string Purpose { get; set; }
}

public class GetItemRequestsRequest
{
    public ItemRequestStatus? Status { get; set; }
    public Guid? DepartmentId { get; set; }
}

public class RejectItemRequestRequest
{
    public string Reason { get; set; }
}

