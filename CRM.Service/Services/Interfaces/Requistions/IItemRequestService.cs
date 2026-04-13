using CRM.Base.Common.Services.Interface;

namespace CRM.Services.Interfaces;

public interface IItemRequestService : IMSSQLBaseService<ItemRequest, Guid>
{
    Task<Result<ItemRequestResponse>> ApproveAsync(Guid id, ApproveItemRequestRequest request);
    Task<Result<ItemRequestResponse>> RejectAsync(Guid id, string reason);
}