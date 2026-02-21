namespace CRM.API.Middleware;

public class TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        var tenantId = ResolveTenantId(context);

        if (tenantId > 0)
        {
            context.Items["TenantId"] = tenantId;
            logger.LogDebug("Resolved tenant: {TenantId}", tenantId);
        }

        await next(context);
    }

    private static int ResolveTenantId(HttpContext context)
    {
        // 1. JWT custom claim (ideal for production)
        if (int.TryParse(context.User.FindFirst("tenant_id")?.Value, out var fromClaim))
            return fromClaim;

        // 2. Request header (useful during development / multi-tenant testing)
        if (int.TryParse(context.Request.Headers["X-Tenant-Id"].FirstOrDefault(), out var fromHeader))
            return fromHeader;

        // 3. Subdomain parsing (farm1.app.com → resolve via DB)
        var host = context.Request.Host.Host;
        if (host.Contains('.'))
        {
            var subdomain = host.Split('.')[0];
            // Note: This requires a DB lookup — skipped here for simplicity.
            // In production, cache subdomain→tenantId mapping.
        }

        return 0;
    }
}