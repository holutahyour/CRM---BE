using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Services.Interface;

namespace CRM.Services.Interfaces;

public interface IMenuService : IMSSQLBaseService<Menu, Guid>
{
    Task<Result<IEnumerable<MenuDTO>>> GetMyMenusAsync(string oid, Guid tenantId);
}
