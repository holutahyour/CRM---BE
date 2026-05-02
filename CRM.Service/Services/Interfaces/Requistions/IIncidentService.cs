using CRM.Base.Common.Services.Interface;
using CRM.Base.Common.Domain.Common;
using CRM.Domain.DTOs;

namespace CRM.Services.Interfaces;

public interface IIncidentService : IMSSQLBaseService<Incident, Guid>
{
    Task<Result<IncidentResponse>> MarkInProgressAsync(Guid id);
    Task<Result<IncidentResponse>> MarkResolvedAsync(Guid id, string resolution);
}