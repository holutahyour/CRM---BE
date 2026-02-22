using Asp.Versioning;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class TenantController : ControllerBase
{
    private readonly ITenantService _service;

    public TenantController(ITenantService service)
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


