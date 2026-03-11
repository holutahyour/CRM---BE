namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class ActivitiesController : MSSQLBaseController<Activity, ActivityResponse, Guid>
{
    private readonly IActivityService _service;

    public ActivitiesController(IActivityService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateActivity([FromBody] CreateActivityRequest request)
    {

        var response = await CreateAsync(request);

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateActivity(Guid id, [FromBody] UpdateActivityRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveActivity(Guid id)
    {

        var response = await RemoveAsync(id);

        return Ok(response);
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateActivityRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }
}
