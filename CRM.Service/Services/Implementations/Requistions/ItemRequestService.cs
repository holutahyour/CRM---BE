namespace CRM.Services.Implementations;

public class ItemRequestService : MSSQLBaseService<ItemRequest, Guid>, IItemRequestService
{
    private readonly IMSSQLRepository<ItemRequest, Guid> _baseRepository;
    private readonly IMSSQLRepository<Activity, Guid> _activityRepository;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMSSQLRepository<Item, Guid> _itemRepository;

    public ItemRequestService(
    IMSSQLRepository<ItemRequest, Guid> baseRepository,
    IMSSQLRepository<Activity, Guid> activityRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IMSSQLRepository<Item, Guid> itemRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
    )
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _baseRepository = baseRepository;
        _context = context;
        _activityRepository = activityRepository;
        _itemRepository = itemRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request)
    {
        if (request is CreateItemRequestRequest createReq)
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
                    var title = dict.ContainsKey("itemName") ? dict["itemName"].ToString() : "Item Request";
                    var activity = new Activity
                    {
                        Type = ActivityType.ItemRequest,
                        Description = $"New Item Request '{title}' submitted",
                        Status = "Pending",
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
                    if (status == "Approved" || status == "Rejected" || status == "Fulfilled" || status == "1" || status == "2" || status == "3")
                    {
                        var activity = new Activity
                        {
                            Type = ActivityType.ItemRequest,
                            Description = $"Item Request marked as {status}",
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

    public async Task<Result<ItemRequestResponse>> ApproveAsync(Guid id)
    {
        Result<ItemRequestResponse> result = new(false);

        try
        {
            var entity = await _baseRepository.GetByIdAsync(id);
            if (entity == null)
            {
                result.SetError("Item request not found", "Item request not found");
                return result;
            }

            entity.Status = ItemRequestStatus.Approved;
            var user = _httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;
            if (user != null)
            {
                entity.ActionedBy = user.Id;
            }
            await _baseRepository.UpdateAsync(id, entity);

            if (entity.ItemId.HasValue)
            {
                var item = await _itemRepository.GetByIdAsync(entity.ItemId.Value);
                if (item != null)
                {
                    item.QuantityOnHand -= entity.Quantity;
                    if (item.QuantityOnHand < 0) item.QuantityOnHand = 0;
                    await _itemRepository.UpdateAsync(item.Id, item);
                }
            }

            try
            {
                var activity = new Activity
                {
                    Type = ActivityType.ItemRequest,
                    Description = $"Item Request '{entity.ItemName}' approved",
                    Status = "Approved",
                    RelatedEntityId = id,
                    DepartmentId = entity.DepartmentId,
                    Timestamp = DateTime.UtcNow
                };
                await _activityRepository.CreateAsync(activity);
            }
            catch { }

            await _context.SaveChangesAsync();

            result.SetSuccess(_mapper.Map<ItemRequestResponse>(entity), $"{entity.ItemName} is approved!");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Item request approval failed");
        }

        return result;
    }

    public async Task<Result<ItemRequestResponse>> RejectAsync(Guid id, string reason)
    {
        Result<ItemRequestResponse> result = new(false);

        try
        {
            var entity = await _baseRepository.GetByIdAsync(id);
            if (entity == null)
            {
                result.SetError("Item request not found", "Item request not found");
                return result;
            }

            entity.Status = ItemRequestStatus.Rejected;
            entity.Reason = reason;
            var user = _httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;
            if (user != null)
            {
                entity.ActionedBy = user.Id;
            }
            await _baseRepository.UpdateAsync(id, entity);

            try
            {
                var activity = new Activity
                {
                    Type = ActivityType.ItemRequest,
                    Description = $"Item Request '{entity.ItemName}' rejected",
                    Status = "Rejected",
                    RelatedEntityId = id,
                    DepartmentId = entity.DepartmentId,
                    Timestamp = DateTime.UtcNow
                };
                await _activityRepository.CreateAsync(activity);
                await _context.SaveChangesAsync();
            }
            catch { }

            result.SetSuccess(_mapper.Map<ItemRequestResponse>(entity), $"{entity.ItemName} is rejected!");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Item request rejection failed");
        }

        return result;
    }
}