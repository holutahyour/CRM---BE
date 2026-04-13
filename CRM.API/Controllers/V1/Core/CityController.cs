using CRM.Domain;

namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class CitiesController : MSSQLBaseController<City, CityResponse, long>
{
    private readonly ICityService _service;

    public CitiesController(ICityService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateCity([FromBody] CreateCityRequest request)
    {

        var response = await CreateAsync(request);

        return response;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateCity(long id, [FromBody] UpdateCityRequest request)
    {

        var response = await UpdateAsync(id, request);

        return response;
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveCity(long id)
    {

        var response = await RemoveAsync(id);

        return response;
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateCityRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return response;
    }
}
