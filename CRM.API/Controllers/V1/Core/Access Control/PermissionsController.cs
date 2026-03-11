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
public class PermissionsController : MSSQLBaseController<Permission, PermissionDTO, Guid>
{
    public PermissionsController(IPermissionService service) : base(service)
    {
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public override async Task<ActionResult> GetAllAsync(
             [FromQuery] string? search = null,
             [FromQuery] string? filter = null,
             [FromQuery] int page = 1,
             [FromQuery] int pageSize = 100,
             [FromQuery] string? select = null,
             [FromQuery] string? orderBy = null,
             [FromQuery] OrderDirectionEnum orderDirection = OrderDirectionEnum.Asc)
    {
        return await base.GetAllAsync(search, filter, page, pageSize, select, orderBy, orderDirection);
    }
}
