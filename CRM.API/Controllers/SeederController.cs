using CRM.Data.Seeds;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiController]
public class SeedersController : ControllerBase
{
    private readonly OperationalDataSeeder _service;

    public SeedersController(OperationalDataSeeder service)
    {
        _service = service;
    }

    [HttpPost("seed-operational-data")]
    public async Task<ActionResult> SeedOperationalData()
    {
        await _service.InitializeAsync();

        return Ok("Operational data seeded successfully");
    }

}
