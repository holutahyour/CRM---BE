using CRM.Domain.Enums;

namespace CRM.Domain.DTOs;

public partial class IncidentResponse : CreateIncidentRequest
{
    public Guid Id { get; set; }
    public string DepartmentName { get; set; }
    public string ReportedByName { get; set; }
    public IncidentStatus Status { get; set; }
    public string Resolution { get; set; }
}

public class CreateIncidentRequest
{
    public string Code { get; set; } = string.Empty;
    public string DeviceType { get; set; }
    public string DeviceId { get; set; }
    public IncidentPriority Priority { get; set; }
    public string IssueDescription { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid? ReportedBy { get; set; }
    public DateTime? Date { get; set; }
}

public class UpdateIncidentRequest
{
    public IncidentStatus? Status { get; set; }
    public string Resolution { get; set; }
}

public class MarkIncidentResolvedRequest
{
    public string Resolution { get; set; }
}

public class IncidentSummary
{
    public int Open { get; set; }
    public int InProgress { get; set; }
    public int Resolved { get; set; }
    public int Critical { get; set; }
}

public class GetIncidentsRequest
{
    public IncidentStatus? Status { get; set; }
    public IncidentPriority? Priority { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
}