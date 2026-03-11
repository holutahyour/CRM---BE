using Asp.Versioning;
using CRM.Base.Common.Presentation;
using CRM.Domain;
using CRM.Domain.Entities;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class EthnicitiesController : MSSQLBaseController<Ethnicity, EthnicityResponse, long>
{
    private readonly IEthnicityService _service;

    public EthnicitiesController(IEthnicityService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateEthnicity([FromBody] CreateEthnicityRequest request)
    {

        var response = await CreateAsync(request);

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateEthnicity(long id, [FromBody] UpdateEthnicityRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveEthnicity(long id)
    {

        var response = await RemoveAsync(id);

        return Ok(response);
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateEthnicityRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }
}
