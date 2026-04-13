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

        return response;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateBatch(Guid id, [FromBody] UpdateBatchRequest request)
    {

        var response = await UpdateAsync(id, request);

        return response;
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveBatch(Guid id)
    {

        var response = await RemoveAsync(id);

        return response;
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateBatchRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return response;
    }
}
