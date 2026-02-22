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
public class UserController : MSSQLBaseController<User, UserDTO, Guid>
{
    private readonly IUserService _service;

    public UserController(IUserService service) : base(service)
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

    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    [Route("me")]
    public async Task<ActionResult> GetMeAsync()
    {
        var oid = GetCurrentUserOid();

        var requestTime = DateTime.UtcNow;
        var response = await _service.GetMeAsync(oid);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;


        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser([FromBody] CreateUserRequest request)
    {

        var response = await CreateAsync(request);

        return Ok(response);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut]
    public async Task<ActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete]
    public async Task<ActionResult> RemoveUser(Guid id)
    {

        var response = await RemoveAsync(id);

        return Ok(response);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateUserRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }
}
