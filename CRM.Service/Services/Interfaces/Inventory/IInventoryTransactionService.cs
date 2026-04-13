using CRM.Base.Common.Services.Interface;

namespace CRM.Services.Interfaces;

public interface IInventoryTransactionService : IMSSQLBaseService<InventoryTransaction, Guid>
{
    Task<Result<bool>> RecordTransactionAsync(
        Guid itemId, 
        CRM.Domain.Enums.TransactionType type,
        decimal quantity, 
        Guid? locationId = null, 
        Guid? batchId = null,
        bool autoAllocate = false,
        string? notes = null);
}