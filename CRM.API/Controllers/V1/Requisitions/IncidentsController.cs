namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class IncidentsController : MSSQLBaseController<Incident, IncidentResponse, Guid>
{
    private readonly IIncidentService _service;

    public IncidentsController(IIncidentService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateIncident([FromBody] CreateIncidentRequest request)
    {

        var response = await CreateAsync(request);

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateIncident(Guid id, [FromBody] UpdateIncidentRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveIncident(Guid id)
    {

        var response = await RemoveAsync(id);

        return Ok(response);
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateIncidentRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }

    [HttpPut("{id}/in-progress")]
    public async Task<ActionResult> MarkInProgress(Guid id)
    {
        var response = await _service.MarkInProgressAsync(id);
        if (response.IsSuccess)
            return Ok(response);
        return BadRequest(response);
    }

    [HttpPut("{id}/resolve")]
    public async Task<ActionResult> MarkResolved(Guid id, [FromBody] MarkIncidentResolvedRequest request)
    {
        var response = await _service.MarkResolvedAsync(id, request.Resolution);
        if (response.IsSuccess)
            return Ok(response);
        return BadRequest(response);
    }
}
