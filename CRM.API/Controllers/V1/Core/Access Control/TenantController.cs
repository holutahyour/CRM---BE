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

    protected async Task<ActionResult> OnboardAsync<TRequest>([FromBody] string name, [FromBody] string? code, [FromBody] string adminEmail)
    {

        var requestTime = DateTime.UtcNow;
        var response = await _service.OnboardAsync(name, code, adminEmail);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;


        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }
}
