# Phase 6 — Multi-Tenancy

> **Goal**: Implement tenant resolution middleware, EF Core global query filters, and tenant-aware data operations.

---

## Step 6.1 — Tenant Middleware

**File**: `src/IMS.API/Middleware/TenantMiddleware.cs`

```csharp
namespace IMS.API.Middleware;

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
```

---

## Step 6.2 — EF Core Global Query Filters

Already implemented in Phase 3's `ImsDbContext.OnModelCreating()`. The key behavior:

- **Every query** on a `TenantEntity` is automatically filtered by `TenantId == currentTenantId && !IsDeleted`
- **Every query** on a `BaseEntity` (non-tenant) is filtered by `!IsDeleted`
- Use `.IgnoreQueryFilters()` when you intentionally need cross-tenant access (e.g., user provisioning, admin operations)

### Testing the Filter

```csharp
// This query automatically includes WHERE tenant_id = @currentTenantId AND is_deleted = 0
var items = await db.Items.ToListAsync();

// To bypass (e.g., super-admin):
var allItems = await db.Items.IgnoreQueryFilters().ToListAsync();
```

---

## Step 6.3 — Automatic Tenant Assignment

The `AuditSaveChangesInterceptor` (Phase 3, Step 3.4) already handles:

```csharp
// In the interceptor's SavingChangesAsync:
foreach (var entry in context.ChangeTracker.Entries<TenantEntity>()
    .Where(e => e.State == EntityState.Added && e.Entity.TenantId == 0))
{
    entry.Entity.TenantId = tenantProvider.TenantId;
}
```

This ensures every new `TenantEntity` gets the correct `TenantId` without the developer needing to set it manually.

---

## Step 6.4 — Tenant Onboarding Flow

When a new organization signs up:

1. **Create Tenant** entry (via a public registration endpoint or admin API)
2. **Seed default roles** for the new tenant (Admin, Manager, Staff)
3. **Activate default modules** (Core + Inventory as minimum)
4. **First user** is JIT-provisioned and assigned the Admin role

**File**: `src/IMS.Application/Features/Tenants/TenantOnboardingService.cs`

```csharp
public class TenantOnboardingService(ImsDbContext db)
{
    public async Task<Tenant> OnboardAsync(string name, string? code, string adminEmail)
    {
        var tenant = new Tenant
        {
            Name = name,
            Code = code,
            SubscriptionStatus = SubscriptionStatus.Trial,
            TrialEndDate = DateTime.UtcNow.AddDays(30)
        };
        db.Tenants.Add(tenant);
        await db.SaveChangesAsync();

        // Seed default roles
        var adminRole = new Role { TenantId = tenant.Id, Name = "Administrator", Code = "ADMIN", IsSystem = true };
        var managerRole = new Role { TenantId = tenant.Id, Name = "Manager", Code = "MANAGER", IsSystem = true };
        var staffRole = new Role { TenantId = tenant.Id, Name = "Staff", Code = "STAFF", IsSystem = true };
        db.Roles.AddRange(adminRole, managerRole, staffRole);

        // Assign all permissions to admin role
        var allPermissions = await db.Permissions.ToListAsync();
        foreach (var perm in allPermissions)
            db.RolePermissions.Add(new RolePermission { TenantId = tenant.Id, RoleId = adminRole.Id, PermissionId = perm.Id });

        // Activate default modules
        var defaultModules = await db.Modules.Where(m => m.Code == "CORE" || m.Code == "INVENTORY").ToListAsync();
        foreach (var mod in defaultModules)
            db.TenantModules.Add(new TenantModule { TenantId = tenant.Id, ModuleId = mod.Id, IsActive = true, ActivatedAt = DateTime.UtcNow });

        await db.SaveChangesAsync();
        return tenant;
    }
}
```

---

## Step 6.5 — Tenant Settings API

**Endpoints**:

| Method | Endpoint                                  | Policy           | Description             |
| ------ | ----------------------------------------- | ---------------- | ----------------------- |
| `GET`  | `/api/tenant/settings`                    | `ManagerOrAbove` | Get current tenant info |
| `PUT`  | `/api/tenant/settings`                    | `AdminOnly`      | Update tenant settings  |
| `GET`  | `/api/tenant/modules`                     | `ManagerOrAbove` | List activated modules  |
| `POST` | `/api/tenant/modules/{moduleId}/activate` | `AdminOnly`      | Activate a module       |

---

## Verification Checklist

- [ ] Different tenants see only their own data
- [ ] New entities auto-receive `TenantId` on save
- [ ] `.IgnoreQueryFilters()` bypasses tenant filter when needed
- [ ] Tenant resolution works via JWT claim, header, or subdomain
- [ ] Onboarding creates tenant + default roles + default modules
- [ ] Changing `X-Tenant-Id` header shows different data (for multi-tenant testing)

---

→ [Phase 7 — Core Module](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/07-Core-Module.md)
