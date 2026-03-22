using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using CRM.Domain.DTOs;
using Microsoft.AspNetCore.Http;

namespace CRM.Services.Implementations;

public class ItemService : MSSQLBaseService<Item, Guid>, IItemService
{
    private readonly IMSSQLRepository<Item, Guid> _repository;

    public ItemService(
    IMSSQLRepository<Item, Guid> baseRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
    )
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _repository = baseRepository;
    }

    public async Task<Result<IList<LowStockItemResponse>>> GetLowStockAsync()
    {
        var result = new Result<IList<LowStockItemResponse>>(false);

        try
        {
            var items = await _repository.GetAllAsync(i => i.MinStockLevel.HasValue && i.QuantityOnHand <= i.MinStockLevel.Value);
            var lowStockItems = items.Select(i => new LowStockItemResponse
            {
                Id = i.Id,
                Name = i.Name,
                MinStockLevel = i.MinStockLevel,
                ReorderQuantity = i.ReorderQuantity,
                QuantityOnHand = i.QuantityOnHand,
                ImageUrl = i.ImageUrl
            }).Take(50).ToList();

            result.SetSuccess(lowStockItems, "Retrieved successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while retrieving records.");

        }

        return result;
    }
}