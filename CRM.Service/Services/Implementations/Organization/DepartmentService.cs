using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using CRM.Base.Enums;
using CRM.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRM.Services.Implementations;

public class DepartmentService : MSSQLBaseService<Department, Guid>, IDepartmentService
{
    private readonly IApplicationDbContext _context;

    public DepartmentService(
    IMSSQLRepository<Department, Guid> baseRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
    )
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _context = context;
    }

    /// <summary>
    /// Returns departments with <see cref="DepartmentResponse.StaffCount"/> overridden to the LIVE
    /// number of users actually assigned to each department (Users.DepartmentId), rather than the
    /// static figure seeded from the import document. The user count is tenant-scoped automatically
    /// by the global query filter, matching the (already tenant-scoped) department rows.
    /// </summary>
    public override async Task<Result<dynamic>> GetAllAsync<TResponse>(
        string? search = null,
        string? filter = null,
        int page = 1,
        int pageSize = 10,
        string? select = null,
        string? orderBy = null,
        OrderDirectionEnum orderDirection = OrderDirectionEnum.Asc,
        string baseUrl = "{app_url}")
    {
        var result = await base.GetAllAsync<TResponse>(search, filter, page, pageSize, select, orderBy, orderDirection, baseUrl);

        // Only adjust the standard department projection (skip the `select`-projected dictionary shape).
        object? content = result.Content;
        if (result.IsSuccess && content is IList<DepartmentResponse> departments && departments.Count > 0)
        {
            var deptIds = departments.Select(d => d.Id).ToList();

            var staffCounts = await _context.Set<User>()
                .Where(u => u.DepartmentId != null && deptIds.Contains(u.DepartmentId.Value))
                .GroupBy(u => u.DepartmentId!.Value)
                .Select(g => new { DepartmentId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DepartmentId, x => x.Count);

            foreach (var dept in departments)
                dept.StaffCount = staffCounts.TryGetValue(dept.Id, out var count) ? count : 0;
        }

        return result;
    }
}
