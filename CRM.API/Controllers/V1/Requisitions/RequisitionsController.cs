using Asp.Versioning;
using CRM.Base.Common.Presentation;
using CRM.Domain.DTOs;
using CRM.Domain.Entities;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class RequisitionsController : MSSQLBaseController<Requisition, RequisitionResponse, Guid>
{
    private readonly IRequisitionService _service;

    public RequisitionsController(IRequisitionService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateRequisition([FromBody] CreateRequisitionRequest request)
    {
        var response = await CreateAsync(request);
        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateRequisition(Guid id, [FromBody] UpdateRequisitionRequest request)
    {
        var response = await UpdateAsync(id, request);
        return Ok(response);
    }

    [HttpPut("{id}/approve")]
    public async Task<ActionResult> ApproveRequisition(Guid id)
    {
        var response = await _service.ApproveAsync(id);
        return Ok(response);
    }

    [HttpPut("{id}/reject")]
    public async Task<ActionResult> RejectRequisition(Guid id, [FromBody] RejectRequisitionRequest request)
    {
        var response = await _service.RejectAsync(id, request.Reason);
        return Ok(response);
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateRequisitionRequest[] requests)
    {
        var response = await ImportAsync(requests);
        return Ok(response);
    }
}
