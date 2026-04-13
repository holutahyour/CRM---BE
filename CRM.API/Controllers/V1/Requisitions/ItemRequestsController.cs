namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class ItemRequestsController : MSSQLBaseController<ItemRequest, ItemRequestResponse, Guid>
{
    private readonly IItemRequestService _service;

    public ItemRequestsController(IItemRequestService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateItemRequest([FromBody] CreateItemRequestRequest request)
    {

        var response = await CreateAsync(request);

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateItemRequest(Guid id, [FromBody] UpdateItemRequestRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    //[HttpDelete]
    //public async Task<ActionResult> RemoveItemRequest(Guid id)
    //{

    //    var response = await RemoveAsync(id);

    //    return Ok(response);
    //}

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateItemRequestRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }

    [HttpPut("{id}/approve")]
    public async Task<ActionResult> ApproveItemRequest(Guid id, [FromBody] ApproveItemRequestRequest request)
    {
        var response = await _service.ApproveAsync(id, request);
        return Ok(response);
    }

    [HttpPut("{id}/reject")]
    public async Task<ActionResult> RejectItemRequest(Guid id, [FromBody] RejectItemRequestRequest request)
    {
        var response = await _service.RejectAsync(id, request.Reason);
        return Ok(response);
    }
}

