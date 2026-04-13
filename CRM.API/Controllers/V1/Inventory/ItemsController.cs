namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class ItemsController : MSSQLBaseController<Item, ItemResponse, Guid>
{
    private readonly IItemService _service;

    public ItemsController(IItemService service) : base(service)
    {
        _service = service;
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult> GetLowStockItems()
    {
        var response = await _service.GetLowStockAsync();
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> CreateItem([FromBody] CreateItemRequest request)
    {

        var response = await CreateAsync(request);

        return response;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateItem(Guid id, [FromBody] UpdateItemRequest request)
    {

        var response = await UpdateAsync(id, request);

        return response;
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveItem(Guid id)
    {

        var response = await RemoveAsync(id);

        return response;
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateItemRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return response;
    }
}
