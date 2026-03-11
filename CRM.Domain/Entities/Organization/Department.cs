using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("org_departments")]
public class Department : TenantEntity<Guid>
{
    public string Name { get; set; } // e.g., Agronomy, Operations
    public string Description { get; set; } // e.g., "Focuses on crop science..."
    public int StaffCount { get; set; } // e.g., 18
    public int ProjectsCount { get; set; } // e.g., 15
    public decimal Budget { get; set; } // e.g., 3500000.00 (in Naira)
    public decimal PercentOfTotal { get; set; } // e.g., 11.4
}
