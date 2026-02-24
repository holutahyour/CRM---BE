using Asp.Versioning;
using CRM.Base.Common.Presentation;
using CRM.Domain.DTOs.Core;
using CRM.Domain.Entities;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class TenantsController : MSSQLBaseController<Tenant, TenantDTO, Guid>
{
    private readonly ITenantService _service;

    public TenantsController(ITenantService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    [Route("onboarding")]
    public async Task<ActionResult> OnboardAsync(TenantOnboardingDTO tenantOnboarding)
    {

        var requestTime = DateTime.UtcNow;
        var response = await _service.OnboardAsync(tenantOnboarding.name, tenantOnboarding.code, tenantOnboarding.adminEmail);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;


        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }
}

public record TenantOnboardingDTO(string name, string? code, string adminEmail);


