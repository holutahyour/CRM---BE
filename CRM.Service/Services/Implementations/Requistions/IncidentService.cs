using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using CRM.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace CRM.Services.Implementations;

public class IncidentService : MSSQLBaseService<Incident, Guid>, IIncidentService
{
    private readonly IMSSQLRepository<Activity, Guid> _activityRepository;
    private readonly IApplicationDbContext _context;

    public IncidentService(
    IMSSQLRepository<Incident, Guid> baseRepository,
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
                    var deviceType = dict.ContainsKey("deviceType") ? dict["deviceType"].ToString() : "Device";
                    var activity = new Activity
                    {
                        Type = ActivityType.Incident,
                        Description = $"New Incident reported for {deviceType}",
                        Status = "Open",
                        RelatedEntityId = dict.ContainsKey("id") && dict["id"] != null ? Guid.Parse(dict["id"].ToString()) : null,
                        DepartmentId = dict.ContainsKey("departmentId") && dict["departmentId"] != null ? Guid.Parse(dict["departmentId"].ToString()) : null,
                        Timestamp = DateTime.UtcNow
                    };
                    await _activityRepository.CreateAsync(activity);
                    await _context.SaveChangesAsync();
                }
            }
            catch { /* Ignore logging errors */ }
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
                    if (status == "Resolved" || status == "Closed" || status == "2" || status == "3")
                    {
                        var activity = new Activity
                        {
                            Type = ActivityType.Incident,
                            Description = $"Incident marked as {status}",
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