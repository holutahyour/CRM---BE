# Phase 5c — Integration: User Mapping & JIT Provisioning

> **Goal**: Automate the creation of local user records upon their first login through Azure AD and manage their local role assignments.

---

## Step 1 — JIT User Provisioning Middleware

This middleware ensures that every user authenticated by Azure exists in our local `Users` table.

### 1.1 Implementation Detail (`CRM.API/Middleware`)

Edit `UserProvisioningMiddleware.cs` to handle automatic creation:

```csharp
public async Task InvokeAsync(HttpContext context, ApplicationDbContext db)
{
    if (context.User.Identity?.IsAuthenticated != true) { await next(context); return; }

    var oid = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
    if (oid == null) { await next(context); return; }

    var user = await db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.EntraObjectId == oid);

    if (user == null)
    {
        user = new User
        {
            EntraObjectId = oid,
            Email = context.User.FindFirst("email")?.Value ?? "",
            FirstName = context.User.FindFirst("given_name")?.Value,
            LastName = context.User.FindFirst("family_name")?.Value,
            TenantId = ResolveTenantId(context),
            IsActive = true,
            CreatedOn = DateTime.UtcNow
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }

    context.Items["CurrentUser"] = user;
    await next(context);
}
```

---

## Step 2 — Assigning Local Roles

Because control is now local, you must assign roles to users within the application database.

### 2.1 Default System Roles

The system seeds a default **Administrator** role (`ADMIN`) which is pre-configured with all available permissions.

### 2.2 Role Assignment Process

1.  **Identity Verification**: User logs in via Azure AD.
2.  **Provisioning**: User record is created in `CRM.Domain.Entities.User`.
3.  **Authorization**: An administrator must assign a local `Role` (e.g., `Administrator` or a custom role) to the user via the `UserRoles` join table.

### 2.3 Management Endpoints

- `GET /api/v1/users` — List users.
- `POST /api/v1/roles` — Create custom application roles.
- `POST /api/v1/users/{id}/roles` — Assign local roles to users.

---

## Step 3 — Verification

- [ ] Clear all roles from a user in the database.
- [ ] Attempt to call any protected API (Expected: `403 Forbidden`).
- [ ] Insert a record into `UserRoles` linking the user to the seeded `Administrator` role.
- [ ] Attempt access again (Expected: `200 OK`).

---

→ [Phase 6 — Multi-Tenancy](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/06-Multi-Tenancy.md)
