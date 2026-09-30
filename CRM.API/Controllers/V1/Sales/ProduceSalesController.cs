namespace CRM.API.Controllers.v1;

/// <summary>Fresh Produce sales transactions.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sales/produce/records")]
public class ProduceSalesController
    : MSSQLBaseController<ProduceSale, ProduceSaleResponse, Guid>
{
    public ProduceSalesController(IProduceSaleService service) : base(service)
    {
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateProduceSaleRequest request)
        => await CreateAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult> Remove(Guid id) => await RemoveAsync(id);
}
