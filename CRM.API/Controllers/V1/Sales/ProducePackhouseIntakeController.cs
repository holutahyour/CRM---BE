namespace CRM.API.Controllers.v1;

/// <summary>Fresh Produce packhouse intake and grading.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sales/produce/intake")]
public class ProducePackhouseIntakeController
    : MSSQLBaseController<ProducePackhouseIntake, ProducePackhouseIntakeResponse, Guid>
{
    public ProducePackhouseIntakeController(IProducePackhouseIntakeService service) : base(service)
    {
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateProducePackhouseIntakeRequest request)
        => await CreateAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult> Remove(Guid id) => await RemoveAsync(id);
}
