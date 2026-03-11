using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using Microsoft.AspNetCore.Http;

namespace CRM.Services.Implementations;

public class ItemRequestService : MSSQLBaseService<ItemRequest, Guid>, IItemRequestService
{
    public ItemRequestService(
    IMSSQLRepository<ItemRequest, Guid> baseRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
    )
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {

    }

}