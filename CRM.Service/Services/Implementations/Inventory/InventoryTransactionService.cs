using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using CRM.Domain.Entities;

namespace CRM.Services.Implementations;

public class InventoryTransactionService : MSSQLBaseService<InventoryTransaction, Guid>, IInventoryTransactionService
{
    private readonly IMSSQLRepository<Item, Guid> _itemRepository;
    private readonly IApplicationDbContext _context;

    public InventoryTransactionService(
    IMSSQLRepository<InventoryTransaction, Guid> baseRepository,
    IMSSQLRepository<Item, Guid> itemRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
    )
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _itemRepository = itemRepository;
        _context = context;
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