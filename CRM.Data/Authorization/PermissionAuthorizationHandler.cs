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
        // We use IgnoreQueryFilters on the navigation properties as well to ensure 
        // that the authorization check works even if the TenantProvider isn't fully stable yet
        // and because the user might be in a "System" tenant while checking against a filtered set.
        var hasPermission = await db.Users
            .IgnoreQueryFilters()
            .Where(u => u.EntraObjectId == oid && !u.IsDeleted)
            .AnyAsync(u => u.UserRoles
                .Any(ur => !ur.IsDeleted && ur.Role.RolePermissions
                    .Any(rp => !rp.IsDeleted && rp.Permission.Code == requirement.Permission)));

        if (hasPermission)
            context.Succeed(requirement);
    }
}
