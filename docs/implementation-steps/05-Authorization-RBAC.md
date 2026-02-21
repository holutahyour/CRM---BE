# Phase 5 — Authorization & RBAC

> **Goal**: Implement permission-based authorization policies, handlers, and role management APIs.

---

## Step 5.1 — Authorization Policies in `Program.cs`

```csharp
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", p => p.RequireRole("Admin"))
    .AddPolicy("ManagerOrAbove", p => p.RequireRole("Admin", "Manager"))
    .AddPolicy("SupervisorOrAbove", p => p.RequireRole("Admin", "Manager", "Supervisor"))
    .AddPolicy("InventoryRead", p => p.AddRequirements(new PermissionRequirement(Permissions.ItemsView)))
    .AddPolicy("InventoryWrite", p => p.AddRequirements(new PermissionRequirement(Permissions.ItemsCreate)))
    .AddPolicy("PurchaseOrderApprove", p => p.AddRequirements(new PermissionRequirement(Permissions.PurchaseOrdersApprove)))
    .AddPolicy("ReportsView", p => p.AddRequirements(new PermissionRequirement(Permissions.ReportsView)));

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
```

---

## Step 5.2 — Permission Requirement & Handler

**File**: `src/IMS.Application/Common/Authorization/PermissionRequirement.cs`

```csharp
namespace IMS.Application.Common.Authorization;

using Microsoft.AspNetCore.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
```

**File**: `src/IMS.Infrastructure/Authorization/PermissionAuthorizationHandler.cs`

```csharp
namespace IMS.Infrastructure.Authorization;

using IMS.Application.Common.Authorization;
using IMS.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

public class PermissionAuthorizationHandler(ImsDbContext db) : AuthorizationHandler<PermissionRequirement>
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
```

---

## Step 5.3 — Permission Caching (Optional Performance Optimization)

**File**: `src/IMS.Infrastructure/Authorization/CachedPermissionService.cs`

```csharp
namespace IMS.Infrastructure.Authorization;

using IMS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public interface IPermissionService
{
    Task<IReadOnlySet<string>> GetUserPermissionsAsync(string entraOid);
    Task InvalidateUserPermissionsAsync(string entraOid);
}

public class CachedPermissionService(ImsDbContext db, IDistributedCache cache) : IPermissionService
{
    private const int CacheMinutes = 15;

    public async Task<IReadOnlySet<string>> GetUserPermissionsAsync(string entraOid)
    {
        var cacheKey = $"permissions:{entraOid}";
        var cached = await cache.GetStringAsync(cacheKey);

        if (cached != null)
            return JsonSerializer.Deserialize<HashSet<string>>(cached)!;

        var permissions = await db.Users
            .IgnoreQueryFilters()
            .Where(u => u.EntraObjectId == entraOid && !u.IsDeleted)
            .SelectMany(u => u.UserRoles.Where(ur => !ur.IsDeleted))
            .SelectMany(ur => ur.Role.RolePermissions.Where(rp => !rp.IsDeleted))
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync();

        var set = permissions.ToHashSet();
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(set),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheMinutes) });

        return set;
    }

    public async Task InvalidateUserPermissionsAsync(string entraOid)
    {
        await cache.RemoveAsync($"permissions:{entraOid}");
    }
}
```

Register: `services.AddScoped<IPermissionService, CachedPermissionService>();`

---

## Step 5.4 — Role Management DTOs & Validators

**File**: `src/IMS.Application/Features/Roles/DTOs/RoleDto.cs`

```csharp
public record RoleDto(int Id, string Name, string Code, string? Description, bool IsSystem, bool IsActive, List<string> Permissions);
public record CreateRoleRequest(string Name, string Code, string? Description, List<int> PermissionIds);
public record UpdateRoleRequest(string Name, string? Description, bool IsActive, List<int> PermissionIds);
```

**File**: `src/IMS.Application/Features/Roles/Validators/CreateRoleValidator.cs`

```csharp
public class CreateRoleValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50)
            .Matches("^[A-Z_]+$").WithMessage("Code must be uppercase with underscores only");
        RuleFor(x => x.PermissionIds).NotEmpty();
    }
}
```

---

## Step 5.5 — Role Controller

**File**: `src/IMS.API/Controllers/RolesController.cs`

Endpoints:

- `GET /api/roles` — list all roles for tenant (with permissions)
- `POST /api/roles` — create role + assign permissions
- `PUT /api/roles/{id}` — update role + reassign permissions
- `DELETE /api/roles/{id}` — soft-delete (block if `IsSystem`)
- `GET /api/permissions` — list all available permissions (global)

All endpoints require `[Authorize(Policy = "AdminOnly")]` except `GET /api/permissions`.

---

## Verification Checklist

- [ ] `[Authorize(Policy = "AdminOnly")]` blocks non-admin users (403)
- [ ] `[Authorize(Policy = "InventoryRead")]` checks DB permission for user
- [ ] Admin role (`roles` JWT claim) bypasses all permission checks
- [ ] Permission cache populates on first check, invalidates on role change
- [ ] Role CRUD operations work with permission assignment
- [ ] System roles (`IsSystem = true`) cannot be deleted

---

→ [Phase 6 — Multi-Tenancy](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/06-Multi-Tenancy.md)
