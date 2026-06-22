using CRM.Data.Seeds;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiController]
public class SeedersController : ControllerBase
{
    private readonly OperationalDataSeeder _service;
    private readonly InventoryDataSeeder _inventorySeeder;
    private readonly WorkflowDataSeeder _workflowSeeder;

    public SeedersController(OperationalDataSeeder service, InventoryDataSeeder inventorySeeder, WorkflowDataSeeder workflowSeeder)
    {
        _service = service;
        _inventorySeeder = inventorySeeder;
        _workflowSeeder = workflowSeeder;
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

    [HttpPost("seed-workflow-data")]
    public async Task<ActionResult> SeedWorkflowData()
    {
        var summary = await _workflowSeeder.InitializeAsync();

        return Ok(summary);
    }

}
