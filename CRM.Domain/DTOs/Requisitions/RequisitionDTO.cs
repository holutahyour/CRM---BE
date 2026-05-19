using CRM.Domain.Enums;

namespace CRM.Domain.DTOs;


public partial class RequisitionResponse : CreateRequisitionRequest
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; } // e.g., "2026-02-03"
    public RequisitionStatus Status { get; set; }
    public string Reason { get; set; } // For rejected
    public Guid? SubmittedBy { get; set; } // User Id
    public string SubmittedByName { get; set; }
    public string DepartmentName { get; set; }
    public Guid? ActionedBy { get; set; }
    public string ActionedByName { get; set; }
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
    public Guid? DepartmentId { get; set; }
    public Guid? SubmittedBy { get; set; }
    public string? FileUrl { get; set; }
    public string? FileOriginalName { get; set; }
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
