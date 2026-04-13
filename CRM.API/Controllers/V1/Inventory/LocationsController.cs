namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class LocationsController : MSSQLBaseController<Location, LocationResponse, Guid>
{
    private readonly ILocationService _service;

    public LocationsController(ILocationService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateLocation([FromBody] CreateLocationRequest request)
    {

        var response = await CreateAsync(request);

        return response;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateLocation(Guid id, [FromBody] UpdateLocationRequest request)
    {

        var response = await UpdateAsync(id, request);

        return response;
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveLocation(Guid id)
    {

        var response = await RemoveAsync(id);

        return response;
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateLocationRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return response;
    }
}
