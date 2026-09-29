using CRM.Data.Seeds;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiController]
public class SeedersController : ControllerBase
{
    private readonly OperationalDataSeeder _service;
    private readonly InventoryDataSeeder _inventorySeeder;
    private readonly WorkflowDataSeeder _workflowSeeder;
    private readonly ProcessingDataSeeder _processingSeeder;

    public SeedersController(OperationalDataSeeder service, InventoryDataSeeder inventorySeeder, WorkflowDataSeeder workflowSeeder, ProcessingDataSeeder processingSeeder)
    {
        _service = service;
        _inventorySeeder = inventorySeeder;
        _workflowSeeder = workflowSeeder;
        _processingSeeder = processingSeeder;
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

    [HttpPost("seed-processing-data")]
    public async Task<ActionResult> SeedProcessingData()
    {
        var summary = await _processingSeeder.InitializeAsync();

        return Ok(summary);
    }

}
