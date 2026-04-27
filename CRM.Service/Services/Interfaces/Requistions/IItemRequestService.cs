namespace CRM.Services.Interfaces;

public interface IItemRequestService : IMSSQLBaseService<ItemRequest, Guid>
{
    Task<Result<ItemRequestResponse>> ApproveAsync(Guid id);
    Task<Result<ItemRequestResponse>> RejectAsync(Guid id, string reason);
}