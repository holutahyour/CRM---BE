namespace CRM.Domain.DTOs;

public partial class MonthlyReportResponse : CreateMonthlyReportRequest
{
    public Guid Id { get; set; } // User Id

    public decimal Progress { get; set; } // e.g., 65 (percentage)
    public string SubmittedByName { get; set; }
    public string DepartmentName { get; set; }
}

public partial class CreateMonthlyReportRequest
{
    public string Code { get; set; } = string.Empty;
    public string Month { get; set; } // e.g., "February"
    public int Year { get; set; } // e.g., 2026
    public string GoalTitle { get; set; } // e.g., "Lead Generation"
    public decimal TargetValue { get; set; } // e.g., 100
    public decimal AchievedValue { get; set; } // e.g., 65
    public string Notes { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid? SubmittedBy { get; set; } // User Id

}

public partial class UpdateMonthlyReportRequest
{
    public decimal? AchievedValue { get; set; }
    public string Notes { get; set; }
}


public class GetMonthlyReportsRequest
{
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string Month { get; set; }
    public int? Year { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
}

public class GetMonthlyReportsSummaryRequest
{
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
}

public class MonthlyReportsSummary
{
    public decimal AverageAchievement { get; set; } // Percentage
    public List<GraphData> Graphs { get; set; } // For bar graphs, etc.
}

public class GraphData
{
    public string Label { get; set; }
    public decimal Value { get; set; }
}
