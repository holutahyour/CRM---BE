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

    //[HttpDelete]
    //public async Task<ActionResult> RemoveRequisition(Guid id)
    //{

    //    var response = await RemoveAsync(id);

    //    return Ok(response);
    //}

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateRequisitionRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }
}

