using Asp.Versioning;
using CRM.Base.Common.Presentation;
using CRM.Domain.DTOs;
using CRM.Domain.Entities;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class InventoryTransactionController : MSSQLBaseController<InventoryTransaction, InventoryTransactionResponse, Guid>
{
    private readonly IInventoryTransactionService _service;

    public InventoryTransactionController(IInventoryTransactionService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateInventoryTransaction([FromBody] CreateInventoryTransactionRequest request)
    {

        var response = await CreateAsync(request);

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateInventoryTransaction(Guid id, [FromBody] UpdateInventoryTransactionRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveInventoryTransaction(Guid id)
    {

        var response = await RemoveAsync(id);

        return Ok(response);
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateInventoryTransactionRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }
}
