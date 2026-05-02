namespace CRM.Services.Implementations;

public class IncidentService : MSSQLBaseService<Incident, Guid>, IIncidentService
{
    private readonly IMSSQLRepository<Incident, Guid> _baseRepository;
    private readonly IMSSQLRepository<Activity, Guid> _activityRepository;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

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
        _baseRepository = baseRepository;
        _activityRepository = activityRepository;
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request)
    {
        if (request is CreateIncidentRequest createReq)
        {
            var user = _httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;
            if (user != null)
            {
                createReq.ReportedBy = user.Id;
            }
        }

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

    public async Task<Result<IncidentResponse>> MarkInProgressAsync(Guid id)
    {
        Result<IncidentResponse> result = new(false);
        try
        {
            var entity = await _baseRepository.GetByIdAsync(id);
            if (entity == null)
            {
                result.SetError("Incident not found", "Incident not found");
                return result;
            }

            entity.Status = IncidentStatus.InProgress;
            await _baseRepository.UpdateAsync(id, entity);

            try
            {
                var activity = new Activity
                {
                    Type = ActivityType.Incident,
                    Description = $"Incident '{entity.DeviceType} - {entity.DeviceId}' marked as In Progress",
                    Status = "In Progress",
                    RelatedEntityId = id,
                    DepartmentId = entity.DepartmentId,
                    Timestamp = DateTime.UtcNow
                };
                await _activityRepository.CreateAsync(activity);
                await _context.SaveChangesAsync();
            }
            catch { }

            result.SetSuccess(_mapper.Map<IncidentResponse>(entity), "Incident marked as In Progress");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Failed to mark as In Progress");
        }
        return result;
    }

    public async Task<Result<IncidentResponse>> MarkResolvedAsync(Guid id, string resolution)
    {
        Result<IncidentResponse> result = new(false);
        try
        {
            var entity = await _baseRepository.GetByIdAsync(id);
            if (entity == null)
            {
                result.SetError("Incident not found", "Incident not found");
                return result;
            }

            entity.Status = IncidentStatus.Resolved;
            entity.Resolution = resolution;
            await _baseRepository.UpdateAsync(id, entity);

            try
            {
                var activity = new Activity
                {
                    Type = ActivityType.Incident,
                    Description = $"Incident '{entity.DeviceType} - {entity.DeviceId}' marked as Resolved",
                    Status = "Resolved",
                    RelatedEntityId = id,
                    DepartmentId = entity.DepartmentId,
                    Timestamp = DateTime.UtcNow
                };
                await _activityRepository.CreateAsync(activity);
                await _context.SaveChangesAsync();
            }
            catch { }

            result.SetSuccess(_mapper.Map<IncidentResponse>(entity), "Incident marked as Resolved");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Failed to mark as Resolved");
        }
        return result;
    }
}