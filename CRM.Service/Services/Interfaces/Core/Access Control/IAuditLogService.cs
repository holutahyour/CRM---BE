using CRM.Base.Common.Domain.Common;
using CRM.Domain.DTOs.Core;

namespace CRM.Services.Interfaces;

public interface IAuditLogService
{
    /// <summary>Returns a paginated, filtered page of audit logs scoped to <paramref name="currentTenantId"/>
    /// (unless <see cref="AuditLogFilter.AllTenants"/> is set by a super-admin caller).</summary>
    Task<Result<IList<AuditLogDTO>>> GetAllAsync(AuditLogFilter filter, Guid currentTenantId, string baseUrl);

    /// <summary>Returns the filtered audit logs (capped) serialized as CSV bytes.</summary>
    Task<Result<byte[]>> ExportCsvAsync(AuditLogFilter filter, Guid currentTenantId);
}
