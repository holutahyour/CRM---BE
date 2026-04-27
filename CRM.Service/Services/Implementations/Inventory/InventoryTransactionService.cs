namespace CRM.Services.Implementations;

public class InventoryTransactionService : MSSQLBaseService<InventoryTransaction, Guid>, IInventoryTransactionService
{
    private readonly IMSSQLRepository<InventoryTransaction, Guid> _baseRepository;
    private readonly IMSSQLRepository<Item, Guid> _itemRepository;
    private readonly IMSSQLRepository<ItemLocation, Guid> _itemLocationRepository;
    private readonly IMSSQLRepository<Batch, Guid> _batchRepository;
    private readonly IApplicationDbContext _context;

    public InventoryTransactionService(
    IMSSQLRepository<InventoryTransaction, Guid> baseRepository,
    IMSSQLRepository<Item, Guid> itemRepository,
    IMSSQLRepository<ItemLocation, Guid> itemLocationRepository,
    IMSSQLRepository<Batch, Guid> batchRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
    )
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _baseRepository = baseRepository;
        _itemRepository = itemRepository;
        _itemLocationRepository = itemLocationRepository;
        _batchRepository = batchRepository;
        _context = context;
    }

    public async Task<Result<bool>> RecordTransactionAsync(
        Guid itemId,
        TransactionType type,
        decimal quantity,
        Guid? locationId = null,
        Guid? batchId = null,
        bool autoAllocate = false,
        string? notes = null)
    {
        Result<bool> result = new(false);
        try
        {
            var item = await _itemRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                result.SetError("Item not found", "Item not found");
                return result;
            }

            bool isAdd = type == TransactionType.Purchase ||
                         type == TransactionType.TransferIn ||
                         type == TransactionType.Return ||
                         type == TransactionType.Production;

            if (type == TransactionType.Adjustment && quantity > 0) isAdd = true;
            if (type == TransactionType.Adjustment && quantity <= 0)
            {
                isAdd = false;
                quantity = Math.Abs(quantity);
            }

            // Simple implementation for Manual/Auto (In a full implementation, auto Allocate fetches locations/batches via FIFO)
            // For now, mapping directly if provided, optionally checking ItemLocations.
            var transaction = new InventoryTransaction
            {
                ItemId = itemId,
                TransactionType = type,
                Quantity = quantity,
                LocationId = locationId,
                BatchId = batchId,
                Notes = notes ?? (autoAllocate ? "Auto Allocated" : "Manual Allocation"),
                TransactionDate = DateTime.UtcNow
            };

            await _baseRepository.CreateAsync(transaction);

            // Update item global stock
            decimal change = isAdd ? quantity : -quantity;
            item.QuantityOnHand += change;
            if (item.QuantityOnHand < 0) item.QuantityOnHand = 0;

            await _itemRepository.UpdateAsync(itemId, item);

            // Detailed ItemLocation and Batch tracking would apply changes here
            await _context.SaveChangesAsync();

            result.SetSuccess(true, "Transaction recorded successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.Message, "Failed to record transaction");
        }
        return result;
    }

    public override async Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request)
    {
        var result = await base.CreateAsync<TResponse, TRequest>(request);
        if (result.IsSuccess && result.Content != null)
        {
            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(result.Content));
                if (dict != null && dict.ContainsKey("itemId") && dict.ContainsKey("transactionType") && dict.ContainsKey("quantity"))
                {
                    var itemIdStr = dict["itemId"]?.ToString();
                    var transTypeStr = dict["transactionType"]?.ToString();
                    var quantityStr = dict["quantity"]?.ToString();

                    if (Guid.TryParse(itemIdStr, out Guid itemId) && decimal.TryParse(quantityStr, out decimal quantity))
                    {
                        var item = await _itemRepository.GetByIdAsync(itemId);
                        if (item != null)
                        {
                            bool isAdd = false;
                            if (int.TryParse(transTypeStr, out int transTypeInt))
                            {
                                // 1=Purchase, 3=TransferIn, 7=Return, 8=Production
                                isAdd = (transTypeInt == 1 || transTypeInt == 3 || transTypeInt == 7 || transTypeInt == 8);
                                if (transTypeInt == 5) isAdd = true; // Adjustment
                            }
                            else if (transTypeStr != null)
                            {
                                isAdd = transTypeStr.Contains("Purchase") || transTypeStr.Contains("In") || transTypeStr.Contains("Return") || transTypeStr.Contains("Production") || transTypeStr.Contains("Adjustment");
                            }

                            decimal change = isAdd ? quantity : -quantity;
                            item.QuantityOnHand += change;

                            await _itemRepository.UpdateAsync(itemId, item);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            catch { /* Ignore non-critical logic failures */ }
        }
        return result;
    }
}