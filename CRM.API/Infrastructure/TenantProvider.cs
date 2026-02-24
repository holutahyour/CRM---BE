using CRM.Services.Services.Interfaces.Common;
using System.Security.Claims;

namespace CRM.API.Infrastructure;

public class TenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider
{
    public Guid TenantId
    {
        get
        {
            // 1. Try to get from HttpContext.Items (populated by TenantMiddleware)
            if (httpContextAccessor.HttpContext?.Items.TryGetValue("TenantId", out var tenantId) == true && tenantId is Guid guid)
            {
                return guid;
            }

            // 2. Fallback to JWT claim directly
            var claimValue = httpContextAccessor.HttpContext?.User.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(claimValue, out var fromClaim))
            {
                return fromClaim;
            }

            // 3. Fallback to X-Tenant-Id header
            var headerValue = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            if (Guid.TryParse(headerValue, out var fromHeader))
            {
                return fromHeader;
            }

            return Guid.Empty;
        }
    }

    public string? UserId => httpContextAccessor.HttpContext?.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
}
