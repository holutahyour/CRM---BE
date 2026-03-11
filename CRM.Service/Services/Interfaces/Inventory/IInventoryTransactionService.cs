using CRM.Base.Common.Services.Interface;

namespace CRM.Services.Interfaces;

public interface IInventoryTransactionService : IMSSQLBaseService<InventoryTransaction, Guid>
{
}