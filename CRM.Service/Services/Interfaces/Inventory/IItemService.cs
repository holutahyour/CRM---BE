using CRM.Base.Common.Domain.Common;
using CRM.Domain.DTOs;

namespace CRM.Services.Interfaces;

public interface IItemService : IMSSQLBaseService<Item, Guid>
{
    Task<Result<IList<LowStockItemResponse>>> GetLowStockAsync();
}