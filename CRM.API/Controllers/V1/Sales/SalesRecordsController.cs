namespace CRM.API.Controllers.v1;

/// <summary>Egg sales transactions.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sales/records")]
public class SalesRecordsController : MSSQLBaseController<SalesRecord, SalesRecordResponse, Guid>
{
    public SalesRecordsController(ISalesRecordService service) : base(service)
    {
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateSalesRecordRequest request)
        => await CreateAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult> Remove(Guid id) => await RemoveAsync(id);
}
