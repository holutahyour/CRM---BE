namespace CRM.Domain.DTOs;

public partial class DepartmentResponse : UpdateDepartmentRequest
{
}

public partial class CreateDepartmentRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } // e.g., Agronomy, Operations
    public string Description { get; set; } // e.g., "Focuses on crop science..."
    public int StaffCount { get; set; } // e.g., 18
    public int ProjectsCount { get; set; } // e.g., 15
    public decimal Budget { get; set; } // e.g., 3500000.00 (in Naira)
    public decimal PercentOfTotal { get; set; } // e.g., 11.4

}

public partial class UpdateDepartmentRequest : CreateDepartmentRequest
{
    public required Guid Id { get; set; }
}

public class GetDepartmentsRequest
{
    public string Search { get; set; }
}

public class DepartmentSummary
{
    public int TotalDepartments { get; set; }
    public int TotalStaff { get; set; }
    public int TotalProjects { get; set; }
    public decimal TotalBudget { get; set; }
}

public class GetRecentActivitiesRequest
{
    public int Limit { get; set; } = 5;
}
