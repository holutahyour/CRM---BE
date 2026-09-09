namespace CRM.API.Controllers.v1;

/// <summary>Daily egg stock movement for the Sales department.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sales/stock")]
public class SalesStockController
    : MSSQLBaseController<SalesStockRecord, SalesStockRecordResponse, Guid>
{
    public SalesStockController(ISalesStockRecordService service) : base(service)
    {
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateSalesStockRecordRequest request)
        => await CreateAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult> Remove(Guid id) => await RemoveAsync(id);
}
