using Asp.Versioning;
using CRM.Base.Common.Presentation;
using CRM.Domain.DTOs;
using CRM.Domain.Entities;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class BatchesController : MSSQLBaseController<Batch, BatchResponse, Guid>
{
    private readonly IBatchService _service;

    public BatchesController(IBatchService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateBatch([FromBody] CreateBatchRequest request)
    {

        var response = await CreateAsync(request);

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateBatch(Guid id, [FromBody] UpdateBatchRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveBatch(Guid id)
    {

        var response = await RemoveAsync(id);

        return Ok(response);
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateBatchRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }
}
