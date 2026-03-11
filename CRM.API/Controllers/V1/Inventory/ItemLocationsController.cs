using Asp.Versioning;
using CRM.Base.Common.Presentation;
using CRM.Domain.DTOs;
using CRM.Domain.Entities;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class ItemLocationsController : MSSQLBaseController<ItemLocation, ItemLocationResponse, Guid>
{
    private readonly IItemLocationService _service;

    public ItemLocationsController(IItemLocationService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateItemLocation([FromBody] CreateItemLocationRequest request)
    {

        var response = await CreateAsync(request);

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateItemLocation(Guid id, [FromBody] UpdateItemLocationRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveItemLocation(Guid id)
    {

        var response = await RemoveAsync(id);

        return Ok(response);
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateItemLocationRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }
}
