namespace CRM.Domain.DTOs;

public partial class DashboardSummaryResponse
{
    public int PendingRequisitions { get; set; }
    public int ApprovedRequisitions { get; set; }
    public int ItemRequests { get; set; }
    public int LowStock { get; set; }
    public int OpenIncidents { get; set; }
    public decimal MonthlyGoalsAchieved { get; set; } // Percentage


}

