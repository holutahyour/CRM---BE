using Asp.Versioning;
using CRM.Base.Common.Presentation;
using CRM.Domain;
using CRM.Domain.Entities;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateCity(long id, [FromBody] UpdateCityRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveCity(long id)
    {

        var response = await RemoveAsync(id);

        return Ok(response);
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateCityRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }
}
