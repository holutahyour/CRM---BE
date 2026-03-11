using Asp.Versioning;
using CRM.Base.Common.Presentation;
using CRM.Domain;
using CRM.Domain.Entities;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class StatesController : MSSQLBaseController<State, StateResponse, long>
{
    private readonly IStateService _service;

    public StatesController(IStateService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateState([FromBody] CreateStateRequest request)
    {

        var response = await CreateAsync(request);

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateState(long id, [FromBody] UpdateStateRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    //[HttpDelete]
    //public async Task<ActionResult> RemoveState(long id)
    //{

    //    var response = await RemoveAsync(id);

    //    return Ok(response);
    //}

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateStateRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }
}

