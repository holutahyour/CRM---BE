using CRM.Base.Enums;
using CRM.Domain.DTOs.Core;
using Microsoft.AspNetCore.Authorization;

namespace CRM.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class MenusController : MSSQLBaseController<Menu, MenuDTO, Guid>
{
    private readonly IMenuService _service;
    private const string RoleAdmin = "Admin";

    public MenusController(IMenuService service) : base(service)
    {
        _service = service;
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public override async Task<ActionResult> GetAllAsync(
             [FromQuery] string? search = null,   // Generic search across all properties
             [FromQuery] string? filter = null,  // Specific filtering (e.g., "Department=Accounting")
             [FromQuery] int page = 1,           // Page number
             [FromQuery] int pageSize = 100,      // Items per page
             [FromQuery] string? select = null,
             [FromQuery] string? orderBy = null,
             [FromQuery] OrderDirectionEnum orderDirection = OrderDirectionEnum.Asc)
    {
        return await base.GetAllAsync(search, filter, page, pageSize, select, orderBy, orderDirection);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    [Route("{id}")]
    public override async Task<ActionResult> GetByIdAsync(Guid id)
    {
        return await base.GetByIdAsync(id);
    }
    //[Authorize(Policy = "AdminOnly")]
    [AllowAnonymous]
    [HttpGet]
    [Route("my-menus")]
    public async Task<ActionResult> GetMyMenusAsync()
    {
        var oid = GetCurrentUserOid();
        var tenantId = GetCurrentTenantId();
        var requestTime = DateTime.UtcNow;
        var response = await _service.GetMyMenusAsync(oid, tenantId);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;


        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<ActionResult> CreateMenu([FromBody] CreateMenuRequest request)
    {
        var response = await CreateAsync(request);
        return Ok(response);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut]
    public async Task<ActionResult> UpdateMenu(Guid id, [FromBody] UpdateMenuRequest request)
    {
        var response = await UpdateAsync(id, request);
        return Ok(response);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete]
    public async Task<ActionResult> RemoveMenu(Guid id)
    {
        var response = await RemoveAsync(id);
        return Ok(response);
    }
}
