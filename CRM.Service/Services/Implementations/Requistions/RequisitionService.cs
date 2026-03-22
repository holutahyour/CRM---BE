namespace CRM.Services.Implementations;

public class RequisitionService : MSSQLBaseService<Requisition, Guid>, IRequisitionService
{
    private readonly IMSSQLRepository<Requisition, Guid> _baseRepository;
    private readonly IMSSQLRepository<Activity, Guid> _activityRepository;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

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
        _baseRepository = baseRepository;
        _activityRepository = activityRepository;
        _context = context;
        _mapper = mapper;
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
                        Type = Domain.Enums.ActivityType.Requistion,
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
                            Type = Domain.Enums.ActivityType.Requistion,
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

    public async Task<Result<RequisitionResponse>> ApproveAsync(Guid id)
    {
        Result<RequisitionResponse> result = new(false);

        try
        {
            var requisition = await GetByIdAsync<RequisitionResponse>(id);
            if (!requisition.IsSuccess || requisition.Content == null)
                result.SetError($"Requisition not found", "Requisition not found");

            var updateResult = await base.UpdateAsync<UpdateRequisitionRequest>(
                id,
                new UpdateRequisitionRequest
                {
                    Title = requisition.Content.Title,
                    Amount = requisition.Content.Amount,
                    Description = requisition.Content.Description,
                }
            );

            // Update status directly via the base repository
            var entity = await _baseRepository.GetByIdAsync(id);
            if (entity != null)
            {
                entity.Status = RequisitionStatus.Approved;
                await _baseRepository.UpdateAsync(id, entity);

                try
                {
                    var activity = new Activity
                    {
                        Type = Domain.Enums.ActivityType.Requistion,
                        Description = $"Requisition '{entity.Title}' approved",
                        Status = "Approved",
                        RelatedEntityId = id,
                        DepartmentId = entity.DepartmentId,
                        Timestamp = DateTime.UtcNow
                    };
                    await _activityRepository.CreateAsync(activity);
                }
                catch { }

                await _context.SaveChangesAsync();
            }

            result.SetSuccess(_mapper.Map<RequisitionResponse>(entity), $"{entity.Title} is approved!");

            return await GetByIdAsync<RequisitionResponse>(id);
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), $"Requisition approval failed");
        }

        return result;
    }

    public async Task<Result<RequisitionResponse>> RejectAsync(Guid id, string reason)
    {
        Result<RequisitionResponse> result = new(false);

        try
        {
            var entity = await _baseRepository.GetByIdAsync(id);
            if (entity == null)
                result.SetError($"Requisition not found", "Requisition not found");

            entity.Status = RequisitionStatus.Rejected;
            entity.Reason = reason;
            await _baseRepository.UpdateAsync(id, entity);

            try
            {
                var activity = new Activity
                {
                    Type = Domain.Enums.ActivityType.Requistion,
                    Description = $"Requisition '{entity.Title}' rejected",
                    Status = "Rejected",
                    RelatedEntityId = id,
                    DepartmentId = entity.DepartmentId,
                    Timestamp = DateTime.UtcNow
                };
                await _activityRepository.CreateAsync(activity);
                await _context.SaveChangesAsync();
            }
            catch { }

            result.SetSuccess(_mapper.Map<RequisitionResponse>(entity), $"{entity.Title} is rejected!");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), $"Requisition rejection failed");
        }

        return result;
    }
}