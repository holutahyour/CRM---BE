using CRM.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Middleware;

public class UserProvisioningMiddleware(RequestDelegate next, ILogger<UserProvisioningMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, ApplicationDbContext db, IConfiguration config)
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

            try
            {
                await db.SaveChangesAsync();
                logger.LogInformation("JIT provisioned user {Email} for tenant {TenantId}", user.Email, tenantId);

                await AssignAdminRoleAsync(user, db);
            }
            catch (DbUpdateException)
            {
                // A concurrent authenticated request (common on first login: several parallel
                // calls all see "no user" and race to provision) already inserted this Entra
                // user, tripping the unique IX_users_EntraObjectId index. Recover idempotently:
                // detach our failed insert and reuse the record the winning request created.
                db.Entry(user).State = EntityState.Detached;

                var existing = await db.Users.IgnoreQueryFilters()
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.EntraObjectId == oid);

                // If no such user exists, the failure was not a duplicate-provisioning race —
                // surface the original error rather than masking a real problem.
                if (existing == null) throw;

                user = existing;
                logger.LogInformation("User {Oid} was provisioned by a concurrent request; reusing existing record.", oid);
            }

            //// Auto-assign Admin role to the first system user
            //if (isFirstUser)
            //{
            //    await AssignAdminRoleAsync(user, db);
            //}
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

        // Ensure designated owner account(s) carry the SUPER_ADMIN role. The permission handler
        // reads roles from the DB on every request, so granting it here takes effect immediately
        // (no re-login). Configured via Auth:SuperAdminEmails in appsettings.
        var superAdminEmails = config.GetSection("Auth:SuperAdminEmails").Get<string[]>() ?? [];
        if (!string.IsNullOrWhiteSpace(user.Email) &&
            superAdminEmails.Any(e => string.Equals(e, user.Email, StringComparison.OrdinalIgnoreCase)))
        {
            await EnsureSuperAdminRoleAsync(user, db);
        }

        context.Items["CurrentUser"] = user;
        context.Items["oid"] = user.EntraObjectId;
        context.Items["TenantId"] = user.TenantId;
        await next(context);
    }

    private async Task EnsureSuperAdminRoleAsync(User user, ApplicationDbContext db)
    {
        var superRole = await db.Roles.IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Code == "SUPER_ADMIN" && (r.TenantId == user.TenantId || r.TenantId == Guid.Empty));
        if (superRole == null)
        {
            logger.LogWarning("SUPER_ADMIN role not found; cannot elevate {Email}", user.Email);
            return;
        }

        var alreadyHas = await db.UserRoles.IgnoreQueryFilters()
            .AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == superRole.Id && !ur.IsDeleted);
        if (alreadyHas) return;

        db.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = superRole.Id,
            TenantId = user.TenantId
        });
        await db.SaveChangesAsync();
        logger.LogInformation("Granted SUPER_ADMIN to designated owner {Email}", user.Email);
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
