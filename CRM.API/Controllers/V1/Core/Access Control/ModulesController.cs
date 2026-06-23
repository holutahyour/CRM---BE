using Asp.Versioning;
using CRM.Domain.DTOs.Core;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ModulesController : ControllerBase
{
    private readonly IModuleService _service;

    public ModulesController(IModuleService service)
    {
        _service = service;
    }

    /// <summary>Global module catalog (with category names).</summary>
    [Authorize(Policy = "ModulesView")]
    [HttpGet]
    public async Task<ActionResult> GetCatalog()
    {
        var result = await _service.GetCatalogAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>Modules enabled/disabled for the current tenant.</summary>
    [Authorize(Policy = "ModulesView")]
    [HttpGet("tenant")]
    public async Task<ActionResult> GetTenantModules()
    {
        var result = await _service.GetTenantModulesAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>Enable or disable a module for the current tenant.</summary>
    [Authorize(Policy = "ModulesManage")]
    [HttpPost("toggle")]
    public async Task<ActionResult> Toggle([FromBody] ToggleModuleRequest request)
    {
        var result = await _service.ToggleAsync(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // --- Super-admin: global catalog management ---

    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] ModuleCatalogRequest request)
    {
        var result = await _service.CreateAsync<ModuleDTO, ModuleCatalogRequest>(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] ModuleCatalogRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Policy = "SuperAdminOnly")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Remove(Guid id)
    {
        var result = await _service.RemoveAsync(id);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
