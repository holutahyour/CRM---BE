using CRM.Base.Domain.Entities;
using CRM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("req_incidents")]
public class Incident : TenantEntity<Guid>
{
    public string DeviceType { get; set; } // e.g., "Laptop", "Printer", "Monitor", "Server"
    public string DeviceId { get; set; } // e.g., "LP-1234"
    public IncidentPriority Priority { get; set; } // Low/Medium/High/Critical
    public string IssueDescription { get; set; }
    public Guid DepartmentId { get; set; }
    public DateTime Date { get; set; } // e.g., "2026-02-03"
    public IncidentStatus Status { get; set; } // Open/InProgress/Resolved
    public string Resolution { get; set; }
    public Guid? ReportedBy { get; set; } // User Id

    public Department Department { get; set; }
}
