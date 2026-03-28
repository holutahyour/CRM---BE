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
        return response;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateRequisition(Guid id, [FromBody] UpdateRequisitionRequest request)
    {
        var response = await UpdateAsync(id, request);
        return response;
    }

    [HttpPut("{id}/approve")]
    public async Task<ActionResult> ApproveRequisition(Guid id)
    {
        var requestTime = DateTime.UtcNow;
        var response = await _service.ApproveAsync(id);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;


        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }

    [HttpPut("{id}/reject")]
    public async Task<ActionResult> RejectRequisition(Guid id, [FromBody] RejectRequisitionRequest request)
    {

        var requestTime = DateTime.UtcNow;
        var response = await _service.RejectAsync(id, request.Reason);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;


        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateRequisitionRequest[] requests)
    {

        var response = await ImportAsync(requests);
        return response;
    }
}
