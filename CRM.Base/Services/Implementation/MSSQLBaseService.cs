using CRM.Base.Enums;

namespace CRM.Base.Common.Services.Implementation;

public abstract class MSSQLBaseService<TEntity, TId> : IMSSQLBaseService<TEntity, TId>
     where TEntity : BaseEntity<TId>
{
    private readonly IMSSQLRepository<TEntity, TId> _baseRepository;
    private readonly IMSSQLRepository<AuditLog, long> _auditLogRepository;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected MSSQLBaseService(
        IMSSQLRepository<TEntity, TId> baseRepository,
        IMSSQLRepository<AuditLog, long> auditLogRepository,
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _baseRepository = baseRepository;
        _auditLogRepository = auditLogRepository;
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public virtual async Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request)
    {
        var result = new Result<TResponse>(false);

        try
        {
            var entity = _mapper.Map<TEntity>(request);

            if (entity != null)
                entity.Code = RandomGenerator.RandomString(10);

            var response = await _baseRepository.CreateAsync(entity);

            var auditLog = new AuditLog
            {
                Code = RandomGenerator.RandomString(10),
                ActionType = ActionType.Create.ToString(),
                EntityName = typeof(TEntity).Name,
                UserId = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "SYSTEM",
                Timestamp = DateTime.UtcNow,
                OldValues = null,
                NewValues = JsonSerializer.Serialize(entity),
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
            };

            await _auditLogRepository.CreateAsync(auditLog);

            await _context.SaveChangesAsync();

            if (response == null)
            {
                result.SetError($"{typeof(TEntity).Name} not created", $"{typeof(TEntity).Name} not created");
            }
            else
            {
                result.SetSuccess(_mapper.Map<TResponse>(response), $"{typeof(TEntity).Name} created successfully!");
            }
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), $"Error while creating {typeof(TEntity).Name}");
        }

        return result;
    }


    public virtual async Task<Result<IList<TResponse>>> GetAllAsync<TResponse>()
    {
        var result = new Result<IList<TResponse>>(false);

        try
        {
            var response = await _baseRepository.GetAllAsync();
            result.SetSuccess(_mapper.Map<IList<TResponse>>(response), "Retrieved successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while retrieving records.");
        }

        return result;
    }
    public virtual async Task<Result<dynamic>> GetAllAsync<TResponse>(
        string search = null,
        string filter = null,
        int page = 1,
        int pageSize = 10,
        string select = null,
        string orderBy = null,
        OrderDirectionEnum orderDirection = OrderDirectionEnum.Asc,
        string baseUrl = "{app_url}")
    {
        Result<dynamic> result = new(false);

        try
        {
            var response = await _baseRepository.GetAllWithMetaAsync(search, filter, page, pageSize, orderBy, orderDirection, baseUrl);

            var responseDTO = _mapper.Map<IList<TResponse>>(response.Content);

            result.SetMeta(
                response.MetaData.Total,
                response.MetaData.From,
                response.MetaData.To,
                response.MetaData.PerPage,
                response.MetaData.LastPage,
                response.MetaData.Path,
                response.MetaData.FirstPageUrl,
                response.MetaData.PrevPageUrl,
                response.MetaData.NextPageUrl,
                response.MetaData.LastPageUrl
                );

            // Selecting specific properties
            if (!string.IsNullOrEmpty(select))
            {
                var selectedProperties = select.Split(',', StringSplitOptions.TrimEntries);
                var selectedData = responseDTO.Select(s => SelectProperties(s, selectedProperties)).ToList();


                result.SetSuccess(selectedData, "Retrieved Successfully.");

                return result;
            }

            result.SetSuccess(responseDTO, "Retrieved Successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.Message, $"Error while retrieving {typeof(TEntity).Name}");

            return result;
        }
        return result;
    }


    public virtual async Task<Result<TResponse>> GetByIdAsync<TResponse>(TId id)
    {
        Result<TResponse> result = new(false);

        try
        {
            var response = await _baseRepository.GetByIdAsync(id);

            result.SetSuccess(_mapper.Map<TResponse>(response), "Retrieved Successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while retrieving Base");
        }

        return result;
    }

    public virtual async Task<Result<bool>> RemoveAsync(TId id)
    {
        var result = new Result<bool>(false);

        try
        {
            var existingEntity = await _baseRepository.GetByIdAsync(id);
            var oldValues = JsonSerializer.Serialize(existingEntity);

            var response = await _baseRepository.DeleteAsync(id);

            await _context.SaveChangesAsync();

            if (!response)
                result.SetError($"{typeof(TEntity).Name} not deleted", $"{typeof(TEntity).Name} with Id {id} not deleted");

            else
            {
                var auditLog = new AuditLog
                {
                    Code = RandomGenerator.RandomString(10),
                    ActionType = ActionType.Delete.ToString(),
                    EntityName = typeof(TEntity).Name,
                    UserId = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "SYSTEM",
                    Timestamp = DateTime.UtcNow,
                    OldValues = oldValues,
                    NewValues = null,
                    IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                };

                await _auditLogRepository.CreateAsync(auditLog);

                await _context.SaveChangesAsync();

                result.SetSuccess(response, $"{typeof(TEntity).Name} with Id {id} deleted successfully!");
            }
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), $"Error while removing {typeof(TEntity).Name}");
        }

        return result;
    }

    public virtual async Task<Result<bool>> UpdateAsync<TRequest>(TId id, TRequest request)
    {
        var result = new Result<bool>(false);

        try
        {
            var existingEntity = await _baseRepository.GetByIdAsync(id);
            var oldValues = JsonSerializer.Serialize(existingEntity);

            if (existingEntity == null)
            {
                result.SetError($"{typeof(TEntity).Name} not updated", $"Record with Id {id} not found.");
                return result;
            }

            var updatedEntity = _mapper.Map(request, existingEntity);
            var newValues = JsonSerializer.Serialize(updatedEntity);

            var auditLog = new AuditLog
            {
                Code = RandomGenerator.RandomString(10),
                ActionType = ActionType.Update.ToString(),
                EntityName = typeof(TEntity).Name,
                UserId = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "SYSTEM",
                Timestamp = DateTime.UtcNow,
                OldValues = oldValues,
                NewValues = newValues,
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
            };

            await _auditLogRepository.CreateAsync(auditLog);

            await _context.SaveChangesAsync();

            result.SetSuccess(true, $"{typeof(TEntity).Name} with Id {id} updated successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while updating record.");
        }

        return result;
    }

    public virtual async Task<Result<bool>> UpdateAsync<TRequest>(IList<TRequest> entities)
    {
        Result<bool> result = new(false);

        try
        {
            foreach (var item in entities)
            {
                var entity = _mapper.Map<TEntity>(item);

                await UpdateAsync(entity.Id, item);
                await _context.SaveChangesAsync();
            }

            result.SetSuccess(true, $"{typeof(TEntity).Name} updated Successfully.");

        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while Updating Base");
        }
        return result;
    }

    public virtual async Task<Result<string>> ImportAsync<TRequest>(TRequest[] entities)
    {
        Result<string> result = new(false);

        try
        {
            if (entities == null || entities.Length == 0)
            {
                result.SetError($"{typeof(TEntity).Name}s not imported", "No data provided for import");
            }
            else
            {
                var mappedData = _mapper.Map<TEntity[]>(entities);

                foreach (var data in mappedData)
                {
                    if (!data.Equals(null)) { data.Code ??= RandomGenerator.RandomString(10); }
                }

                var response = await _baseRepository.AddEntitiesAsync(mappedData);

                await _context.SaveChangesAsync();

                result.SetSuccess("Successful", $"{typeof(TEntity).Name}s  imported Successfully !");
            }

        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), $"Error while Importing {typeof(TEntity).Name}");
        }

        return result;
    }

    public virtual async Task<Result<TResponse[]>> AddEntitiesAsync<TResponse, TRequest>(TRequest[] requests)
    {
        Result<TResponse[]> result = new(false);
        try
        {
            var entities = _mapper.Map<TEntity[]>(requests);
            foreach (var entity in entities)
            {
                if (!entity.Equals(null)) { entity.Code ??= RandomGenerator.RandomString(10); }
            }
            var response = await _baseRepository.AddEntitiesAsync(entities);
            await _context.SaveChangesAsync();
            if (response == null || response.Count() == 0)
            {
                result.SetError($"{typeof(TEntity).Name}s not created", $"{typeof(TEntity).Name}s not created");
            }
            else
            {
                result.SetSuccess(_mapper.Map<TResponse[]>(response), $"{typeof(TEntity).Name}s Created Successfully !");
            }
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), $"Error while creating {typeof(TEntity).Name}");
        }
        return result;
    }

    protected Dictionary<string, object> SelectProperties<T>(T entity, string[] properties)
    {
        var result = new Dictionary<string, object>();
        foreach (var property in properties)
        {
            var propInfo = typeof(T).GetProperty(property, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (propInfo != null)
            {
                result[propInfo.Name.ToCamelCase()] = propInfo.GetValue(entity);
            }
        }
        return result;
    }
}
