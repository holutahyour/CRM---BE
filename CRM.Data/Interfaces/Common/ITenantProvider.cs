namespace CRM.Services.Services.Interfaces.Common;

public interface ITenantProvider
{
    int TenantId { get; }
    string? UserId { get; }  // Entra OID
}
