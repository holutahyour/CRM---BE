namespace CRM.API.Controllers.v1;

/// <summary>Feed purchases for the Sales department.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sales/feed-costs")]
public class SalesFeedCostsController : MSSQLBaseController<SalesFeedCost, SalesFeedCostResponse, Guid>
{
    public SalesFeedCostsController(ISalesFeedCostService service) : base(service)
    {
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateSalesFeedCostRequest request)
        => await CreateAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult> Remove(Guid id) => await RemoveAsync(id);
}
