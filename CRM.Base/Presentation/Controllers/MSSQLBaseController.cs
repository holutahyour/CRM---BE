using CRM.Base.Enums;
using Microsoft.Graph.Models;

namespace CRM.Base.Common.Presentation;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class MSSQLBaseController<TEntity, TResponse, TId> : ControllerBase
{
    private readonly IMSSQLBaseService<TEntity, TId> _service;
    protected Guid GetCurrentTenantId() => HttpContext.Items["TenantId"] is Guid id ? id : Guid.Empty;
    protected string? GetCurrentUserOid() => HttpContext.Items["oid"] is string id ? id : string.Empty;
    protected User? GetCurrentUser() => HttpContext.Items["CurrentUser"] as User;

    public MSSQLBaseController(IMSSQLBaseService<TEntity, TId> service)
    {
        _service = service;
    }

    [HttpGet]
    public virtual async Task<ActionResult> GetAllAsync(
             [FromQuery] string? search = null,   // Generic search across all properties
             [FromQuery] string? filter = null,  // Specific filtering (e.g., "Department=Accounting")
             [FromQuery] int page = 1,           // Page number
             [FromQuery] int pageSize = 100,      // Items per page
             [FromQuery] string? select = null,
             [FromQuery] string? orderBy = null,
             [FromQuery] OrderDirectionEnum orderDirection = OrderDirectionEnum.Asc)
    {
        var request = HttpContext.Request;

        var baseUrl = $"{request.Scheme}://{request.Host}{request.Path}";

        var requestTime = DateTime.UtcNow;
        var response = await _service.GetAllAsync<TResponse>(search, filter, page, pageSize, select, orderBy, orderDirection, baseUrl);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;

        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual async Task<ActionResult> GetByIdAsync(TId id)
    {
        var requestTime = DateTime.UtcNow;
        var response = await _service.GetByIdAsync<TResponse>(id);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;

        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }

    protected async Task<ActionResult> CreateAsync<TRequest>([FromBody] TRequest request)
    {

        var requestTime = DateTime.UtcNow;
        var response = await _service.CreateAsync<TResponse, TRequest>(request);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;


        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }

    protected async Task<ActionResult> UpdateAsync<TRequest>(TId id, [FromBody] TRequest request)
    {
        var requestTime = DateTime.UtcNow;
        var response = await _service.UpdateAsync(id, request);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;


        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }

    protected async Task<ActionResult> UpdateAsync<TRequest>(List<TRequest> request)
    {
        var requestTime = DateTime.UtcNow;
        var response = await _service.UpdateAsync(request);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;


        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }

    protected async Task<ActionResult> AddEntitiesAsync<TRequest>([FromBody] TRequest[] request)
    {

        var requestTime = DateTime.UtcNow;
        var response = await _service.AddEntitiesAsync<TResponse, TRequest>(request);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;


        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }

    protected async Task<ActionResult> RemoveAsync(TId id)
    {
        var requestTime = DateTime.UtcNow;
        var response = await _service.RemoveAsync(id);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;


        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }

    protected async Task<ActionResult> ImportAsync<TRequest>([FromBody] TRequest[] requests)
    {
        var requestTime = DateTime.UtcNow;
        var response = await _service.ImportAsync(requests);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;


        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }

    internal Error PopulateError(int code, string message, string type)
    {
        return new Error()
        {
            Code = code,
            Message = message,
            Type = type
        };
    }
}