using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using Microsoft.AspNetCore.Http;

namespace CRM.Services.Implementations;

public class StateService : MSSQLBaseService<State, long>, IStateService
{
    public StateService(
    IMSSQLRepository<State, long> baseRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
) : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
    }
}