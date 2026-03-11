using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("dash_dashboard_summaries")]
public class DashboardSummary : TenantEntity<Guid>
{
    public int PendingRequisitions { get; set; }
    public int ApprovedRequisitions { get; set; }
    public int ItemRequests { get; set; }
    public int LowStock { get; set; }
    public int OpenIncidents { get; set; }
    public decimal MonthlyGoalsAchieved { get; set; } // Percentage
}
