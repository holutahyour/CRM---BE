namespace CRM.Services.Implementations;

public class RequisitionService : MSSQLBaseService<Requisition, Guid>, IRequisitionService
{
    private readonly IMSSQLRepository<Requisition, Guid> _baseRepository;
    private readonly IMSSQLRepository<Activity, Guid> _activityRepository;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBlobStorageService _blobStorage;
    private readonly IApprovalService _approvalService;

    public RequisitionService(
    IMSSQLRepository<Requisition, Guid> baseRepository,
    IMSSQLRepository<Activity, Guid> activityRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor,
    IBlobStorageService blobStorage,
    IApprovalService approvalService
    )
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _baseRepository = baseRepository;
        _activityRepository = activityRepository;
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _blobStorage = blobStorage;
        _approvalService = approvalService;
    }

    public async Task<Result<RequisitionResponse>> CreateWithFileAsync(
        CreateRequisitionRequest request,
        Stream? fileStream,
        string? fileName,
        string? contentType)
    {
        if (fileStream != null && !string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(contentType))
        {
            var fileUrl = await _blobStorage.UploadAsync(fileStream, fileName, contentType);
            request.FileUrl = fileUrl;
            request.FileOriginalName = fileName;
        }

        return await CreateAsync<RequisitionResponse, CreateRequisitionRequest>(request);
    }

    public override async Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request)
    {
        if (request is CreateRequisitionRequest createReq)
        {
            var user = _httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;
            if (user != null)
            {
                createReq.SubmittedBy = user.Id;
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
            var user = _httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;
            var userId = user?.Id ?? Guid.Empty;
            var tenantId = _httpContextAccessor.HttpContext?.Items["TenantId"] is Guid tid ? tid : Guid.Empty;

            await _approvalService.ApproveAsync(WorkflowType.Requisition, id, userId, tenantId);

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
            var user = _httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;
            var userId = user?.Id ?? Guid.Empty;
            var tenantId = _httpContextAccessor.HttpContext?.Items["TenantId"] is Guid tid ? tid : Guid.Empty;

            await _approvalService.RejectAsync(WorkflowType.Requisition, id, userId, reason, tenantId);

            return await GetByIdAsync<RequisitionResponse>(id);
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), $"Requisition rejection failed");
        }

        return result;
    }
}