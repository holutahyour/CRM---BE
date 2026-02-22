using CRM.Base.Enums;

namespace CRM.Base.Common.Services.Interface;

public interface IMSSQLBaseService<TEntity, TId>
{
    Task<Result<TResponse[]>> AddEntitiesAsync<TResponse, TRequest>(TRequest[] requests);
    Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request);
    Task<Result<IList<TResponse>>> GetAllAsync<TResponse>();
    Task<Result<dynamic>> GetAllAsync<TResponse>(string? search = null, string? filter = null, int page = 1, int pageSize = 10, string? select = null, string? orderBy = null, OrderDirectionEnum orderDirection = OrderDirectionEnum.Asc, string baseUrl = "{app_url}");
    Task<Result<TResponse>> GetByIdAsync<TResponse>(TId id);
    Task<Result<string>> ImportAsync<TRequest>(TRequest[] entities);
    Task<Result<bool>> RemoveAsync(TId id);
    Task<Result<bool>> UpdateAsync<TRequest>(TId id, TRequest entity);
    Task<Result<bool>> UpdateAsync<TRequest>(IList<TRequest> entities);
}
