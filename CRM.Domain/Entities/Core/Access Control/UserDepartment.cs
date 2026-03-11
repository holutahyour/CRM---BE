using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("accl_user_departments")]
public class UserDepartment : TenantEntity<Guid>
{
    public Guid UserId { get; set; }
    public Guid DepartmentId { get; set; }

    public User User { get; set; } = null!;
    public Department Department { get; set; } = null!;
}
