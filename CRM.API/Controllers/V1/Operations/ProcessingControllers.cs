using CRM.Base.Common.Domain.Common;
using Microsoft.AspNetCore.Authorization;

namespace CRM.API.Controllers.v1;

// Operations → Processing record logs (the Batch Production Scheduling workbook). Routes are spelled
// out rather than taken from [controller] because the client groups every Processing tab under one
// operations/ prefix. Reads need operations.view; writes also need operations.manage.

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/operations/products")]
[Authorize(Policy = "OperationsView")]
public class ProcessingProductsController
    : MSSQLBaseController<ProcessingProduct, ProcessingProductResponse, Guid>
{
    public ProcessingProductsController(IProcessingProductService service) : base(service)
    {
    }

    [HttpPost]
    [Authorize(Policy = "OperationsManage")]
    public async Task<ActionResult> Create([FromBody] CreateProcessingProductRequest request)
        => await CreateAsync(request);

    [HttpPut("{id}")]
    [Authorize(Policy = "OperationsManage")]
    public async Task<ActionResult> Update(Guid id, [FromBody] CreateProcessingProductRequest request)
        => await UpdateAsync(id, request);

    [HttpDelete("{id}")]
    [Authorize(Policy = "OperationsManage")]
    public async Task<ActionResult> Remove(Guid id) => await RemoveAsync(id);
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/operations/order-requests")]
[Authorize(Policy = "OperationsView")]
public class OrderRequestsController
    : MSSQLBaseController<OrderRequest, OrderRequestResponse, Guid>
{
    public OrderRequestsController(IOrderRequestService service) : base(service)
    {
    }

    [HttpPost]
    [Authorize(Policy = "OperationsManage")]
    public async Task<ActionResult> Create([FromBody] CreateOrderRequestRequest request)
        => await CreateAsync(request);

    [HttpPut("{id}")]
    [Authorize(Policy = "OperationsManage")]
    public async Task<ActionResult> Update(Guid id, [FromBody] CreateOrderRequestRequest request)
        => await UpdateAsync(id, request);

    [HttpDelete("{id}")]
    [Authorize(Policy = "OperationsManage")]
    public async Task<ActionResult> Remove(Guid id) => await RemoveAsync(id);
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/operations/batches")]
[Authorize(Policy = "OperationsView")]
public class ProductionBatchesController
    : MSSQLBaseController<ProductionBatch, ProductionBatchResponse, Guid>
{
    public ProductionBatchesController(IProductionBatchService service) : base(service)
    {
    }

    [HttpPost]
    [Authorize(Policy = "OperationsManage")]
    public async Task<ActionResult> Create([FromBody] CreateProductionBatchRequest request)
        => await CreateAsync(request);

    [HttpPut("{id}")]
    [Authorize(Policy = "OperationsManage")]
    public async Task<ActionResult> Update(Guid id, [FromBody] CreateProductionBatchRequest request)
        => await UpdateAsync(id, request);

    [HttpDelete("{id}")]
    [Authorize(Policy = "OperationsManage")]
    public async Task<ActionResult> Remove(Guid id) => await RemoveAsync(id);
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/operations/yield-entries")]
[Authorize(Policy = "OperationsView")]
public class YieldEntriesController
    : MSSQLBaseController<YieldEntry, YieldEntryResponse, Guid>
{
    public YieldEntriesController(IYieldEntryService service) : base(service)
    {
    }

    [HttpPost]
    [Authorize(Policy = "OperationsManage")]
    public async Task<ActionResult> Create([FromBody] CreateYieldEntryRequest request)
        => await CreateAsync(request);

    [HttpPut("{id}")]
    [Authorize(Policy = "OperationsManage")]
    public async Task<ActionResult> Update(Guid id, [FromBody] CreateYieldEntryRequest request)
        => await UpdateAsync(id, request);

    [HttpDelete("{id}")]
    [Authorize(Policy = "OperationsManage")]
    public async Task<ActionResult> Remove(Guid id) => await RemoveAsync(id);
}

/// <summary>Material Stock: the workbook's stock cards, read from and posted to Inventory.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/operations/stock-cards")]
[Authorize(Policy = "OperationsView")]
public class StockCardsController : ControllerBase
{
    private readonly IStockCardService _service;

    public StockCardsController(IStockCardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult> List() => Respond(await _service.ListAsync());

    [HttpGet("{itemId}")]
    public async Task<ActionResult> Get(Guid itemId) => Respond(await _service.GetAsync(itemId));

    [HttpPost("{itemId}/receive")]
    [Authorize(Policy = "OperationsManage")]
    public async Task<ActionResult> Receive(Guid itemId, [FromBody] StockMovementRequest request)
        => Respond(await _service.ReceiveAsync(itemId, request));

    [HttpPost("{itemId}/issue")]
    [Authorize(Policy = "OperationsManage")]
    public async Task<ActionResult> Issue(Guid itemId, [FromBody] StockMovementRequest request)
        => Respond(await _service.IssueAsync(itemId, request));

    private ActionResult Respond<T>(Result<T> result) => result.IsSuccess ? Ok(result) : BadRequest(result);
}
