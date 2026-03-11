using CRM.Domain.Entities;

namespace CRM.Services.Interfaces;

public interface IPermissionService : IMSSQLBaseService<Permission, Guid>
{
}
