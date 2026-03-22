using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using CRM.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace CRM.Services.Implementations;

public class RequisitionService : MSSQLBaseService<Requisition, Guid>, IRequisitionService
{
    private readonly IMSSQLRepository<Activity, Guid> _activityRepository;
    private readonly IApplicationDbContext _context;

    public RequisitionService(
    IMSSQLRepository<Requisition, Guid> baseRepository,
    IMSSQLRepository<Activity, Guid> activityRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
    )
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _activityRepository = activityRepository;
        _context = context;
    }

    public override async Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request)
    {
        var result = await base.CreateAsync<TResponse, TRequest>(request);
        if (result.IsSuccess && result.Content != null)
        {
            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(result.Content));
                if (dict != null)
                {
                    var title = dict.ContainsKey("title") ? dict["title"].ToString() : "Requisition";
                    var activity = new Activity
                    {
                        Type = ActivityType.Requistion,
                        Description = $"New Requisition '{title}' submitted",
                        Status = "Pending",
                        RelatedEntityId = dict.ContainsKey("id") && dict["id"] != null ? Guid.Parse(dict["id"].ToString()) : null,
                        DepartmentId = dict.ContainsKey("departmentId") && dict["departmentId"] != null ? Guid.Parse(dict["departmentId"].ToString()) : null,
                        Timestamp = DateTime.UtcNow
                    };
                    await _activityRepository.CreateAsync(activity);
                    await _context.SaveChangesAsync();
                }
            }
            catch { /* Ignore logging errors so we don't break main flow */ }
        }
        return result;
    }

    public override async Task<Result<bool>> UpdateAsync<TRequest>(Guid id, TRequest request)
    {
        var result = await base.UpdateAsync(id, request);
        if (result.IsSuccess)
        {
            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(request));
                if (dict != null && dict.ContainsKey("status"))
                {
                    var status = dict["status"].ToString();
                    // Only log if status changed to a major milestone
                    if (status == "Approved" || status == "Rejected" || status == "1" || status == "2")
                    {
                        var activity = new Activity
                        {
                            Type = ActivityType.Requistion,
                            Description = $"Requisition marked as {status}",
                            Status = status,
                            RelatedEntityId = id,
                            Timestamp = DateTime.UtcNow
                        };
                        await _activityRepository.CreateAsync(activity);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch { }
        }
        return result;
    }
}