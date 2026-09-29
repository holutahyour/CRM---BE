namespace CRM.Services.Interfaces;

// Operations → Processing. The four record logs take the generic CRUD from MSSQLBaseService,
// with request validation layered on in their implementations.

public interface IProcessingProductService : IMSSQLBaseService<ProcessingProduct, Guid>
{
}

public interface IOrderRequestService : IMSSQLBaseService<OrderRequest, Guid>
{
}

public interface IProductionBatchService : IMSSQLBaseService<ProductionBatch, Guid>
{
}

public interface IYieldEntryService : IMSSQLBaseService<YieldEntry, Guid>
{
}

/// <summary>
/// The workbook's stock cards, read from and posted to the Inventory module so Item.QuantityOnHand
/// stays the single stock figure. Only items in <see cref="CRM.Domain.Constants.ProcessingCategories"/>
/// are in scope.
/// </summary>
public interface IStockCardService
{
    Task<Result<IList<StockCardItemResponse>>> ListAsync();
    Task<Result<StockCardResponse>> GetAsync(Guid itemId);
    Task<Result<StockCardResponse>> ReceiveAsync(Guid itemId, StockMovementRequest request);
    Task<Result<StockCardResponse>> IssueAsync(Guid itemId, StockMovementRequest request);
}
