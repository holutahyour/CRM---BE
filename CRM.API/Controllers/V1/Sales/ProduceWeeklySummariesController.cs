namespace CRM.API.Controllers.v1;

/// <summary>Week-by-week Fresh Produce sales aggregates.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sales/produce/weekly-summaries")]
public class ProduceWeeklySummariesController
    : MSSQLBaseController<ProduceWeeklySummary, ProduceWeeklySummaryResponse, Guid>
{
    public ProduceWeeklySummariesController(IProduceWeeklySummaryService service) : base(service)
    {
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateProduceWeeklySummaryRequest request)
        => await CreateAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult> Remove(Guid id) => await RemoveAsync(id);
}
