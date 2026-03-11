using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using CRM.Domain.Entities;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using AutoMapper;

public class PermissionService : MSSQLBaseService<Permission, Guid>, IPermissionService
{
    public PermissionService(
        IMSSQLRepository<Permission, Guid> baseRepository,
        IMSSQLRepository<AuditLog, long> auditLogRepository,
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    )
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
    }
}
