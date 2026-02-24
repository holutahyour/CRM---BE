using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Services.Interface;
using CRM.Domain.DTOs.Core;

namespace CRM.Services.Interfaces;

public interface IUserService : IMSSQLBaseService<User, Guid>
{
    Task<Result<UserRoleDTO>> AssignUserRoleAsync(Guid userId, Guid roleId);
    Task<Result<UserProfileDTO>> GetMeAsync(string oid);
    Task<Result<UserRoleDTO>> RemoveUserRoleAsync(Guid userId, Guid roleId);
}
