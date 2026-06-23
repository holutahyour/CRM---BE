namespace CRM.Domain.DTOs.Core;

public record AuditLogDTO(
    long Id,
    string ActionType,
    string EntityName,
    string UserId,
    DateTime Timestamp,
    string? OldValues,
    string? NewValues,
    string IpAddress,
    string? AdditionalInfo,
    Guid? TenantId);

/// <summary>Query parameters for browsing/exporting audit logs.</summary>
public record AuditLogFilter(
    string? EntityName = null,
    string? ActionType = null,
    string? UserId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 50,
    bool AllTenants = false);
