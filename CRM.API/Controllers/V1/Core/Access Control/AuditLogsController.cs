using Asp.Versioning;
using CRM.Domain.DTOs.Core;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _service;
    private readonly IAuthorizationService _authorization;

    public AuditLogsController(IAuditLogService service, IAuthorizationService authorization)
    {
        _service = service;
        _authorization = authorization;
    }

    private Guid GetCurrentTenantId() => HttpContext.Items["TenantId"] is Guid id ? id : Guid.Empty;

    /// <summary>Only super-admins may bypass tenant scoping.</summary>
    private async Task<bool> ResolveAllTenants(bool requested)
        => requested && (await _authorization.AuthorizeAsync(User, "SuperAdminOnly")).Succeeded;

    [Authorize(Policy = "AuditView")]
    [HttpGet]
    public async Task<ActionResult> GetAllAsync(
        [FromQuery] string? entityName = null,
        [FromQuery] string? actionType = null,
        [FromQuery] string? userId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] bool allTenants = false)
    {
        var filter = new AuditLogFilter(entityName, actionType, userId, fromDate, toDate, page, pageSize,
            await ResolveAllTenants(allTenants));

        var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";
        var result = await _service.GetAllAsync(filter, GetCurrentTenantId(), baseUrl);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Policy = "AuditView")]
    [HttpGet("export")]
    public async Task<ActionResult> ExportAsync(
        [FromQuery] string? entityName = null,
        [FromQuery] string? actionType = null,
        [FromQuery] string? userId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] bool allTenants = false)
    {
        var filter = new AuditLogFilter(entityName, actionType, userId, fromDate, toDate, 1, int.MaxValue,
            await ResolveAllTenants(allTenants));

        var result = await _service.ExportCsvAsync(filter, GetCurrentTenantId());
        if (!result.IsSuccess)
            return BadRequest(result);

        return File(result.Content, "text/csv", $"audit-logs-{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
    }
}
