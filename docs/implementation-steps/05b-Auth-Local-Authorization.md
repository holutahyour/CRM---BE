# Phase 5b — Authorization: Internal Authorization Engine

> **Goal**: Implement a robust, database-driven authorization system that evaluates user permissions locally, independent of Azure Entra ID roles.

---

## Step 1 — Database Schema (`CRM.Domain`)

The authorization engine is built on four core entities located in `CRM.Domain/Entities/Core/Access Control`:

1.  **`User`**: Linked to Azure identity via `EntraObjectId`.
2.  **`Role`**: A collection of permissions (e.g., `Administrator`).
3.  **`Permission`**: A granular action (e.g., `inventory.items.view`).
4.  **`UserRole` / `RolePermission`**: Join entities for many-to-many relationships.

### Define Granular Permissions

Store constant keys in `CRM.Domain/Constants/Access Control/Permissions.cs`:

```csharp
public static class Permissions
{
    public const string ItemsView = "inventory.items.view";
    public const string ItemsCreate = "inventory.items.create";
    public const string UsersManage = "admin.users.manage";
    // ...
}
```

---

## Step 2 — Authorization Handler (`CRM.Data`)

### 2.1 Permission Requirement

The requirement holds the specific permission string needed for access.

### 2.2 The Authorization Handler

The `PermissionAuthorizationHandler` in `CRM.Data/Authorization` performs the lookup against the local database. **Note**: It no longer relies on Azure roles like `Admin`.

```csharp
public class PermissionAuthorizationHandler(ApplicationDbContext db) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var oid = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
        if (oid == null) return;

        var hasPermission = await db.Users
            .IgnoreQueryFilters()
            .Where(u => u.EntraObjectId == oid && !u.IsDeleted)
            .SelectMany(u => u.UserRoles.Where(ur => !ur.IsDeleted))
            .SelectMany(ur => ur.Role.RolePermissions.Where(rp => !rp.IsDeleted))
            .AnyAsync(rp => rp.Permission.Code == requirement.Permission);

        if (hasPermission)
            context.Succeed(requirement);
    }
}
```

---

## Step 3 — Policy Configuration in `Program.cs`

Define named policies using `PermissionRequirement`. **All `RequireRole` calls have been replaced.**

```csharp
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", p => p.AddRequirements(new PermissionRequirement(Permissions.UsersManage)))
    .AddPolicy("ManagerOrAbove", p => p.AddRequirements(new PermissionRequirement(Permissions.ItemsEdit)))
    .AddPolicy("SupervisorOrAbove", p => p.AddRequirements(new PermissionRequirement(Permissions.ItemsView)));

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
```

---

## Step 4 — Verification

- [ ] Log in with a user that has NO local role assigned.
- [ ] Attempt access (Expected: `403 Forbidden`).
- [ ] Assign the local `Administrator` role to the user record in the DB.
- [ ] Attempt access (Expected: `200 OK`).

---

→ [Phase 5c — Integration and User Mapping](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/05c-Auth-User-Role-Mapping.md)
