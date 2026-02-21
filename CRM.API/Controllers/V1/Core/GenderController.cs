using Asp.Versioning;
using CRM.Base.Common.Presentation;
using CRM.Domain;
using CRM.Domain.Entities;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class GendersController : MSSQLBaseController<Gender, GenderResponse, long>
{
    private readonly IGenderService _service;

    public GendersController(IGenderService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateGender([FromBody] CreateGenderRequest request)
    {

        var response = await CreateAsync(request);

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateGender(long id, [FromBody] UpdateGenderRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveGender(long id)
    {

        var response = await RemoveAsync(id);

        return Ok(response);
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateGenderRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }
}
