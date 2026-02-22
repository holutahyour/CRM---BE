namespace CRM.Services.Services.Interfaces.Common;

public interface ITenantProvider
{
    Guid TenantId { get; }
    string? UserId { get; }  // Entra OID
}
