using CRM.Data.Seeds;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiController]
public class SeedersController : ControllerBase
{
    private readonly OperationalDataSeeder _service;
    private readonly InventoryDataSeeder _inventorySeeder;

    public SeedersController(OperationalDataSeeder service, InventoryDataSeeder inventorySeeder)
    {
        _service = service;
        _inventorySeeder = inventorySeeder;
    }

    [HttpPost("seed-operational-data")]
    public async Task<ActionResult> SeedOperationalData()
    {
        await _service.InitializeAsync();

        return Ok("Operational data seeded successfully");
    }

    [HttpPost("seed-inventory-data")]
    public async Task<ActionResult> SeedInventoryData()
    {
        var summary = await _inventorySeeder.InitializeAsync();

        return Ok(summary);
    }

}
