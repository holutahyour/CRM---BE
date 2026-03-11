using CRM.Domain.Enums;

namespace CRM.Domain.DTOs;

public partial class ItemRequestResponse : CreateItemRequestRequest
{
}

public partial class CreateItemRequestRequest
{
    public string Code { get; set; } = string.Empty;
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public string Purpose { get; set; }
    public Guid DepartmentId { get; set; }
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

