using CRM.Base.Common;
using CRM.Domain.Constants;
using CRM.Domain.Validators;
using Microsoft.EntityFrameworkCore;

namespace CRM.Services.Implementations;

/// <summary>
/// The workbook's stock cards (OPENING / RECEIVED / ISSUED OUT / CLOSING / WHERE REQUIRED) as a view
/// over Inventory. Nothing here stores a balance: opening and closing are replayed from the item's
/// transactions and anchored to <see cref="Item.QuantityOnHand"/>, so the card always agrees with
/// the Inventory module.
///
/// Postings bypass <c>InventoryTransactionService.RecordTransactionAsync</c> on purpose: that stamps
/// every transaction with "now" (a stock card is written up after the fact, so the date entered
/// matters) and silently floors stock at zero instead of refusing an over-issue.
/// </summary>
public class StockCardService : IStockCardService
{
    private static readonly StockMovementValidator MovementValidator = new();
    private readonly IApplicationDbContext _context;

    public StockCardService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IList<StockCardItemResponse>>> ListAsync()
    {
        var result = new Result<IList<StockCardItemResponse>>(false);
        try
        {
            var items = await ProcessingItems()
                .OrderBy(i => i.Category!.Name).ThenBy(i => i.Name)
                .ToListAsync();
            result.SetSuccess(items.Select(ToResponse).ToList(), "Retrieved successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while retrieving stock cards.");
        }
        return result;
    }

    public async Task<Result<StockCardResponse>> GetAsync(Guid itemId)
    {
        var result = new Result<StockCardResponse>(false);
        try
        {
            var item = await ProcessingItems().FirstOrDefaultAsync(i => i.Id == itemId);
            if (item == null)
            {
                result.SetError("Stock card not found", "No processing material with that id.");
                return result;
            }
            result.SetSuccess(await BuildCardAsync(item), "Retrieved successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while retrieving the stock card.");
        }
        return result;
    }

    public Task<Result<StockCardResponse>> ReceiveAsync(Guid itemId, StockMovementRequest request)
        => PostAsync(itemId, request, TransactionType.Purchase);

    public Task<Result<StockCardResponse>> IssueAsync(Guid itemId, StockMovementRequest request)
        => PostAsync(itemId, request, TransactionType.Consumption);

    private async Task<Result<StockCardResponse>> PostAsync(
        Guid itemId, StockMovementRequest request, TransactionType type)
    {
        var result = new Result<StockCardResponse>(false);
        try
        {
            var validation = MovementValidator.Validate(request);
            if (!validation.IsValid)
            {
                var error = string.Join(" ", validation.Errors.Select(e => e.ErrorMessage));
                result.SetError(error, error);
                return result;
            }

            var item = await ProcessingItems().FirstOrDefaultAsync(i => i.Id == itemId);
            if (item == null)
            {
                result.SetError("Stock card not found", "No processing material with that id.");
                return result;
            }

            if (type == TransactionType.Consumption && request.Quantity > item.QuantityOnHand)
            {
                var error = $"Cannot issue {request.Quantity:0.####} {item.UnitType} of {item.Name}: "
                          + $"only {item.QuantityOnHand:0.####} {item.UnitType} on hand.";
                result.SetError(error, error);
                return result;
            }

            _context.Set<InventoryTransaction>().Add(new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                Code = RandomGenerator.RandomString(10),
                ItemId = item.Id,
                LocationId = item.LocationId,
                TransactionType = type,
                Quantity = request.Quantity,
                Notes = string.IsNullOrWhiteSpace(request.WhereRequired) ? null : request.WhereRequired.Trim(),
                TransactionDate = request.Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            });
            item.QuantityOnHand += type == TransactionType.Purchase ? request.Quantity : -request.Quantity;

            await _context.SaveChangesAsync();

            var verb = type == TransactionType.Purchase ? "Received" : "Issued";
            result.SetSuccess(await BuildCardAsync(item), $"{verb} {request.Quantity:0.####} {item.UnitType} of {item.Name}.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while posting to the stock card.");
        }
        return result;
    }

    private IQueryable<Item> ProcessingItems() =>
        _context.Set<Item>()
            .Include(i => i.Category)
            .Include(i => i.Location)
            .Where(i => i.Category != null && ProcessingCategories.All.Contains(i.Category.Name));

    private async Task<StockCardResponse> BuildCardAsync(Item item)
    {
        var transactions = await _context.Set<InventoryTransaction>()
            .Where(t => t.ItemId == item.Id)
            .OrderBy(t => t.TransactionDate).ThenBy(t => t.CreatedOn)
            .ToListAsync();

        var movements = transactions.Select(t => (Txn: t, Move: Classify(t))).ToList();

        // Anchor the replay to the current on-hand figure, so stock that predates these transactions
        // (e.g. the earlier inventory import) shows up as the first opening balance.
        var balance = item.QuantityOnHand
                      - movements.Sum(m => m.Move.Received)
                      + movements.Sum(m => m.Move.Issued);

        var rows = new List<StockCardRow>(movements.Count);
        foreach (var (txn, move) in movements)
        {
            var opening = balance;
            balance = opening + move.Received - move.Issued;
            rows.Add(new StockCardRow
            {
                TransactionId = txn.Id,
                Date = DateOnly.FromDateTime(txn.TransactionDate),
                Opening = opening,
                Received = move.Received,
                Issued = move.Issued,
                Closing = balance,
                WhereRequired = txn.Notes,
            });
        }

        return new StockCardResponse { Item = ToResponse(item), Rows = rows };
    }

    /// <summary>Which column of the card a transaction lands in, whatever module posted it.</summary>
    private static (decimal Received, decimal Issued) Classify(InventoryTransaction t) => t.TransactionType switch
    {
        TransactionType.Purchase or TransactionType.TransferIn or TransactionType.Return or TransactionType.Production
            => (t.Quantity, 0m),
        TransactionType.Adjustment when t.Quantity >= 0 => (t.Quantity, 0m),
        TransactionType.Adjustment => (0m, -t.Quantity),
        _ => (0m, t.Quantity),
    };

    private static StockCardItemResponse ToResponse(Item item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Sku = item.Sku,
        CategoryName = item.Category?.Name,
        UnitType = item.UnitType,
        LocationName = item.Location?.Name,
        QuantityOnHand = item.QuantityOnHand,
    };
}
