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
public class ParamaterValuesController : MSSQLBaseController<ParameterValue, ParameterValueResponse, long>
{
    private readonly IParameterValueService _service;

    public ParamaterValuesController(IParameterValueService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateParameterValue([FromBody] CreateParameterValueRequest request)
    {

        var response = await CreateAsync(request);

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateParameterValue(long id, [FromBody] UpdateParameterValueRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveParameterValue(long id)
    {

        var response = await RemoveAsync(id);

        return Ok(response);
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateParameterValueRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }
}
