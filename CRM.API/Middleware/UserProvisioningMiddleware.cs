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
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.EntraObjectId == oid);

        if (user == null)
        {
            var isFirstUser = !await db.Users.IgnoreQueryFilters().AnyAsync();
            var tenantId = await ResolveTenantIdAsync(context, db);

            user = new User
            {
                EntraObjectId = oid,
                Email = context.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? 
                        context.User.FindFirst("email")?.Value ?? 
                        context.User.FindFirst("preferred_username")?.Value ?? 
                        context.User.FindFirst(System.Security.Claims.ClaimTypes.Upn)?.Value ?? "",
                FirstName = context.User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value ?? 
                            context.User.FindFirst("given_name")?.Value ?? 
                            context.User.FindFirst("name")?.Value ?? 
                            context.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value,
                LastName = context.User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value ?? 
                           context.User.FindFirst("family_name")?.Value,
                Phone = context.User.FindFirst(System.Security.Claims.ClaimTypes.MobilePhone)?.Value ??
                        context.User.FindFirst(System.Security.Claims.ClaimTypes.HomePhone)?.Value ??
                        context.User.FindFirst("phone")?.Value ?? "",
                TenantId = tenantId,
                IsActive = true,
                LastLoginAt = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            };
            db.Users.Add(user);
            await db.SaveChangesAsync(); logger.LogInformation("JIT provisioned user {Email} for tenant {TenantId}", user.Email, tenantId);

            // Auto-assign Admin role to the first system user
            if (isFirstUser)
            {
                await AssignAdminRoleAsync(user, db);
            }
        }
        else
        {
            // Self-healing: If user has empty TenantId, assign them to the System tenant
            if (user.TenantId == Guid.Empty)
            {
                var systemTenant = await db.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Code == "SYSTEM");
                if (systemTenant != null)
                {
                    user.TenantId = systemTenant.Id;
                }
                
                // Recovery: If user exists in System tenant but has no roles, assign Admin
                if (!user.UserRoles.Any())
                {
                    await AssignAdminRoleAsync(user, db);
                }
            }

            user.LastLoginAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        context.Items["CurrentUser"] = user;
        context.Items["oid"] = user.EntraObjectId;
        context.Items["TenantId"] = user.TenantId;
        await next(context);
    }

    private async Task AssignAdminRoleAsync(User user, ApplicationDbContext db)
    {
        var adminRole = await db.Roles.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Code == "ADMIN" && (r.TenantId == user.TenantId || r.TenantId == Guid.Empty));
        if (adminRole != null)
        {
            db.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = adminRole.Id,
                TenantId = user.TenantId
            });
            await db.SaveChangesAsync();
            logger.LogInformation("Assigned System Administrator role to user {Email}", user.Email);
        }
    }

    private static async Task<Guid> ResolveTenantIdAsync(HttpContext context, ApplicationDbContext db)
    {
        // 1. Priority: JWT claim
        if (Guid.TryParse(context.User.FindFirst("tenant_id")?.Value, out var fromClaim)) return fromClaim;

        // 2. Request header
        if (Guid.TryParse(context.Request.Headers["X-Tenant-Id"].FirstOrDefault(), out var fromHeader)) return fromHeader;

        // 3. Fallback: Search for "System" tenant by code
        var systemTenant = await db.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Code == "SYSTEM");
        if (systemTenant != null) return systemTenant.Id;

        // 4. Last resort: Any tenant or Guid.Empty (will fail if no tenants exist)
        var firstTenant = await db.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync();
        return firstTenant?.Id ?? Guid.Empty;
    }
}
