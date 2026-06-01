using Asp.Versioning;
using CRM.Base.Common.Presentation;
using CRM.Base.Enums;
using CRM.Domain.DTOs.Core;
using CRM.Domain.Entities;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class RolesController : MSSQLBaseController<Role, RoleDTO, Guid>
{
    private readonly IRoleService _service;

    public RolesController(IRoleService service) : base(service)
    {
        _service = service;
    }

    /// <summary>
    /// Lightweight endpoint returning only id + name pairs.
    /// Accessible to any authenticated user (needed for workflow step assignment dropdowns).
    /// Does NOT expose sensitive permission details.
    /// </summary>
    [Authorize]
    [HttpGet("names")]
    public async Task<ActionResult> GetRoleNames()
    {
        var result = await _service.GetAllAsync<RoleDTO>();
        if (!result.IsSuccess)
            return BadRequest(new { isSuccess = false, content = Array.Empty<object>() });

        var slim = result.Content
            .Select(r => new { id = r.Id, name = r.Name })
            .OrderBy(r => r.name)
            .ToList();

        return Ok(new { isSuccess = true, content = slim });
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

    [HttpPost]
    public async Task<ActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {

        var response = await CreateAsync(request);

        return Ok(response);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut]
    public async Task<ActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete]
    public async Task<ActionResult> RemoveRole(Guid id)
    {

        var response = await RemoveAsync(id);

        return Ok(response);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateRoleRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }
}
