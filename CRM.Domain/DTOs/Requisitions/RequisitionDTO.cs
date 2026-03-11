using CRM.Domain.Enums;

namespace CRM.Domain.DTOs;


public partial class RequisitionResponse : CreateRequisitionRequest
{
    public DateTime Date { get; set; } // e.g., "2026-02-03"
    public RequisitionStatus Status { get; set; }
    public string Reason { get; set; } // For rejected
    public Guid? SubmittedBy { get; set; } // User Id
}

public class GetRequisitionsRequest
{
    public RequisitionStatus? Status { get; set; }
    public Guid? DepartmentId { get; set; }
}

public class CreateRequisitionRequest
{
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public Guid DepartmentId { get; set; }
}

public class UpdateRequisitionRequest
{
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
}

public class RejectRequisitionRequest
{
    public string Reason { get; set; }
}
