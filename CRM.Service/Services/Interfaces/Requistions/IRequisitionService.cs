using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Services.Interface;

namespace CRM.Services.Interfaces;

public interface IRequisitionService : IMSSQLBaseService<Requisition, Guid>
{
    Task<Result<RequisitionResponse>> ApproveAsync(Guid id);
    Task<Result<RequisitionResponse>> RejectAsync(Guid id, string reason);
    Task<Result<RequisitionResponse>> CreateWithFileAsync(CreateRequisitionRequest request, Stream? fileStream, string? fileName, string? contentType);
}