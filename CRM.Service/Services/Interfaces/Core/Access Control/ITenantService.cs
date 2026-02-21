using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Services.Interface;

namespace CRM.Services.Interfaces;

public interface ITenantService : IMSSQLBaseService<Tenant, Guid>
{
    Task<Result<Tenant>> OnboardAsync(string name, string code, string adminEmail);
}
