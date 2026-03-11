using CRM.Base.Domain.Entities;

namespace CRM.Domain.Entities;

// Entity: MonthlyReport
public class MonthlyReport : TenantEntity<Guid>
{
    public string Month { get; set; } // e.g., "February"
    public int Year { get; set; } // e.g., 2026
    public string GoalTitle { get; set; } // e.g., "Lead Generation"
    public decimal TargetValue { get; set; } // e.g., 100
    public decimal AchievedValue { get; set; } // e.g., 65
    public string Notes { get; set; }
    public decimal Progress { get; set; } // e.g., 65 (percentage)
    public Guid DepartmentId { get; set; }
    public Guid? SubmittedBy { get; set; } // User Id

    public Department Department { get; set; }
}
