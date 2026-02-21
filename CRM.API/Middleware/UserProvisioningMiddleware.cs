
using CRM.Data;
using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Middleware;

public class UserProvisioningMiddleware(RequestDelegate next, ILogger<UserProvisioningMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, ApplicationDbContext db)
    {
        if (context.User.Identity?.IsAuthenticated != true) { await next(context); return; }

        var oid = context.User.FindFirst(
            "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
        if (oid == null) { await next(context); return; }

        var user = await db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.EntraObjectId == oid);

        if (user == null)
        {
            var tenantId = ResolveTenantId(context);
            user = new User
            {
                EntraObjectId = oid,
                Email = context.User.FindFirst("email")?.Value ?? context.User.FindFirst("preferred_username")?.Value ?? "",
                FirstName = context.User.FindFirst("given_name")?.Value,
                LastName = context.User.FindFirst("family_name")?.Value,
                TenantId = tenantId,
                IsActive = true,
                LastLoginAt = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            logger.LogInformation("JIT provisioned user {Email} for tenant {TenantId}", user.Email, tenantId);
        }
        else
        {
            user.LastLoginAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        context.Items["CurrentUser"] = user;
        context.Items["TenantId"] = user.TenantId;
        await next(context);
    }

    private static Guid ResolveTenantId(HttpContext context)
    {
        // Priority: JWT claim → Header → Subdomain
        if (Guid.TryParse(context.User.FindFirst("tenant_id")?.Value, out var c)) return c;
        if (Guid.TryParse(context.Request.Headers["X-Tenant-Id"].FirstOrDefault(), out var h)) return h;
        return Guid.Empty;
    }
}
