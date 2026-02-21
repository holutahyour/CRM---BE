# Phase 7 — Core Module: Users, Roles, Menus

> **Goal**: Implement the CRUD APIs and services for user management, role assignment, menu resolution, and module management.

---

## Step 7.1 — User Management

### DTOs

```csharp
public record UserDto(int Id, string Email, string? FirstName, string? LastName, string? Phone, bool IsActive, DateTime? LastLoginAt, List<string> Roles);
public record UserProfileDto(int Id, string Email, string? FirstName, string? LastName, string? Phone, string? AvatarUrl, DateTime? LastLoginAt, List<RoleDto> Roles, List<string> Permissions);
public record UpdateUserRequest(string? FirstName, string? LastName, string? Phone, bool IsActive, List<int>? RoleIds);
```

### Controller: `UsersController`

| Method   | Endpoint                         | Policy           | Description                               |
| -------- | -------------------------------- | ---------------- | ----------------------------------------- |
| `GET`    | `/api/users`                     | `ManagerOrAbove` | List tenant users (paginated, searchable) |
| `GET`    | `/api/users/{id}`                | `ManagerOrAbove` | Get single user with roles                |
| `GET`    | `/api/users/me`                  | `Authenticated`  | Current user profile with permissions     |
| `PUT`    | `/api/users/{id}`                | `AdminOnly`      | Update user info + role assignments       |
| `DELETE` | `/api/users/{id}`                | `AdminOnly`      | Soft-delete user                          |
| `POST`   | `/api/users/{id}/roles`          | `AdminOnly`      | Assign roles to user                      |
| `DELETE` | `/api/users/{id}/roles/{roleId}` | `AdminOnly`      | Remove role from user                     |

### Key Implementation: Get Current User Profile

```csharp
[HttpGet("me")]
public async Task<IActionResult> GetMe()
{
    var oid = GetCurrentUserOid();
    var user = await _db.Users
        .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
        .FirstOrDefaultAsync(u => u.EntraObjectId == oid);

    if (user == null) return NotFound();

    var permissions = user.UserRoles
        .SelectMany(ur => ur.Role.RolePermissions)
        .Select(rp => rp.Permission.Code)
        .Distinct().ToList();

    return Ok(new ApiResponse<UserProfileDto>(true, new UserProfileDto(
        user.Id, user.Email, user.FirstName, user.LastName,
        user.Phone, user.AvatarUrl, user.LastLoginAt,
        user.UserRoles.Select(ur => MapToRoleDto(ur.Role)).ToList(),
        permissions
    )));
}
```

---

## Step 7.2 — Menu Resolution

### DTOs

```csharp
public record MenuDto(int Id, string Name, string Label, string? Icon, string? Route, int Position, List<MenuDto> Children);
```

### Controller: `MenusController`

| Method | Endpoint              | Policy          | Description                         |
| ------ | --------------------- | --------------- | ----------------------------------- |
| `GET`  | `/api/menus/my-menus` | `Authenticated` | Get user's accessible menu tree     |
| `GET`  | `/api/menus`          | `AdminOnly`     | Get all menus (flat list for admin) |
| `POST` | `/api/menus`          | `AdminOnly`     | Create menu item                    |
| `PUT`  | `/api/menus/{id}`     | `AdminOnly`     | Update menu                         |

### Key Implementation: Permission-Filtered Menu Tree

```csharp
[HttpGet("my-menus")]
public async Task<IActionResult> GetMyMenus()
{
    var oid = GetCurrentUserOid();
    var tenantId = GetCurrentTenantId();
    var isAdmin = User.IsInRole("Admin");

    // 1. Get user's permissions
    var userPermissions = isAdmin
        ? new HashSet<string>(await _db.Permissions.Select(p => p.Code).ToListAsync())
        : (await _db.Users
            .Where(u => u.EntraObjectId == oid && u.TenantId == tenantId)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct().ToListAsync())
            .ToHashSet();

    // 2. Get tenant's active modules
    var tenantModules = await _db.TenantModules
        .Where(tm => tm.TenantId == tenantId && tm.IsActive)
        .Select(tm => tm.Module.Code)
        .ToListAsync();

    // 3. Filter menus
    var allMenus = await _db.Menus
        .Where(m => m.IsActive)
        .Where(m => m.ModuleCode == null || tenantModules.Contains(m.ModuleCode))
        .Include(m => m.MenuPermissions).ThenInclude(mp => mp.Permission)
        .OrderBy(m => m.Position)
        .ToListAsync();

    var accessible = allMenus
        .Where(m => !m.MenuPermissions.Any() ||
                     m.MenuPermissions.Any(mp => userPermissions.Contains(mp.Permission.Code)))
        .ToList();

    // 4. Build tree
    var tree = BuildTree(accessible, null);
    return Ok(new ApiResponse<List<MenuDto>>(true, tree));
}

private static List<MenuDto> BuildTree(List<Menu> menus, int? parentId)
{
    return menus
        .Where(m => m.ParentId == parentId)
        .Select(m => new MenuDto(m.Id, m.Name, m.Label, m.Icon, m.Route, m.Position,
            BuildTree(menus, m.Id)))
        .OrderBy(m => m.Position)
        .ToList();
}
```

---

## Step 7.3 — Module Management

### Controller: `ModulesController`

| Method | Endpoint                       | Policy           | Description                               |
| ------ | ------------------------------ | ---------------- | ----------------------------------------- |
| `GET`  | `/api/modules`                 | `ManagerOrAbove` | List all modules (with activation status) |
| `GET`  | `/api/modules/active`          | `Authenticated`  | List active modules for current tenant    |
| `POST` | `/api/modules/{id}/activate`   | `AdminOnly`      | Activate module for tenant                |
| `POST` | `/api/modules/{id}/deactivate` | `AdminOnly`      | Deactivate module for tenant              |

When a module is activated:

1. Create/update `TenantModule` record
2. Invalidate menu cache for all users in tenant

When deactivated:

1. Set `TenantModule.IsActive = false`
2. Users lose access to menus tied to that module (resolved dynamically via `GetMyMenus`)

---

## Step 7.4 — Audit Log Viewer

### Controller: `AuditLogsController`

| Method | Endpoint                                         | Policy                 | Description                     |
| ------ | ------------------------------------------------ | ---------------------- | ------------------------------- |
| `GET`  | `/api/audit-logs`                                | `AuditView` permission | Paginated audit log viewer      |
| `GET`  | `/api/audit-logs/entity/{entityName}/{entityId}` | `AuditView`            | Audit trail for specific entity |

### Query Parameters

```
?page=1&pageSize=50&entityName=Item&action=Update&userId=abc123&from=2026-01-01&to=2026-02-01
```

---

## Step 7.5 — Global Exception Handler

**File**: `src/IMS.API/Middleware/GlobalExceptionHandler.cs`

```csharp
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException e => (StatusCodes.Status404NotFound, e.Message),
            ConflictException e => (StatusCodes.Status409Conflict, e.Message),
            ForbiddenException e => (StatusCodes.Status403Forbidden, e.Message),
            FluentValidation.ValidationException e => (StatusCodes.Status400BadRequest, string.Join("; ", e.Errors.Select(err => err.ErrorMessage))),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "The record was modified by another user. Please refresh and try again."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        if (statusCode == 500) logger.LogError(exception, "Unhandled exception");
        else logger.LogWarning("Handled exception: {Message}", exception.Message);

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new ApiResponse<object>(false, null, message, null), ct);
        return true;
    }
}
```

Register: `builder.Services.AddExceptionHandler<GlobalExceptionHandler>();` and `app.UseExceptionHandler();`

---

## Verification Checklist

- [ ] `GET /api/users/me` returns current user with roles and permissions
- [ ] `GET /api/menus/my-menus` returns filtered tree based on permissions and active modules
- [ ] Admin users see all menus; limited users see only permitted menus
- [ ] Deactivating a module hides its menus from all users
- [ ] Role changes immediately affect menu visibility (after cache expiry)
- [ ] Audit logs are queryable by entity, action, user, and date range
- [ ] Domain exceptions return proper HTTP status codes

---

→ [Phase 8 — Inventory Module](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/08-Inventory-Module.md)
