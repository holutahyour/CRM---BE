namespace CRM.API.Controllers.v1;

/// <summary>
/// Daily egg production for the Sales department.
///
/// The route is spelled out rather than taken from <c>[controller]</c> because
/// the four Sales tabs are grouped under one <c>sales/</c> prefix on the client.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sales/daily-production")]
public class SalesDailyProductionController
    : MSSQLBaseController<SalesDailyProduction, SalesDailyProductionResponse, Guid>
{
    public SalesDailyProductionController(ISalesDailyProductionService service) : base(service)
    {
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateSalesDailyProductionRequest request)
        => await CreateAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult> Remove(Guid id) => await RemoveAsync(id);
}
