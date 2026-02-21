using CRM.Base.Domain.Common;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Data.Authorization;


public interface IPermissionService
{
    Task<IReadOnlySet<string>> GetUserPermissionsAsync(string entraOid);
    Task InvalidateUserPermissionsAsync(string entraOid);
}

public class PermissionAuthorizationHandler(ApplicationDbContext db) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var oid = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
        if (oid == null) return;

        // Check if user's roles contain the required permission
        var hasPermission = await db.Users
            .IgnoreQueryFilters()
            .Where(u => u.EntraObjectId == oid && !u.IsDeleted)
            .SelectMany(u => u.UserRoles.Where(ur => !ur.IsDeleted))
            .SelectMany(ur => ur.Role.RolePermissions.Where(rp => !rp.IsDeleted))
            .AnyAsync(rp => rp.Permission.Code == requirement.Permission);

        // Also check Entra ID Admin role (bypass)
        var isAdmin = context.User.IsInRole("Admin");

        if (hasPermission || isAdmin)
            context.Succeed(requirement);
    }
}
