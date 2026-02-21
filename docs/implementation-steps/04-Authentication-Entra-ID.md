# Phase 4 — Authentication: Azure Entra ID

> **Goal**: Configure Microsoft Entra ID, set up JWT authentication in the API, and implement JIT user provisioning.

---

## Step 4.1 — Azure Portal: App Registration

### Create API App Registration

1. **Microsoft Entra admin center** → **App registrations** → **New registration**
2. Name: `IMS API`, Single tenant, no redirect URI

Note down: **Application (client) ID** and **Directory (tenant) ID**

### Expose API Scopes

Go to **Expose an API** → Set App ID URI → `api://{client-id}`, then add scopes:

| Scope             | Consent          | Description           |
| ----------------- | ---------------- | --------------------- |
| `Inventory.Read`  | Admins and users | Read inventory data   |
| `Inventory.Write` | Admins and users | Modify inventory data |
| `Admin.All`       | Admins only      | Full admin access     |

### Define App Roles

Go to **App roles** → Create:

| Value         | Display Name  | Members      |
| ------------- | ------------- | ------------ |
| `Admin`       | Administrator | Users/Groups |
| `Manager`     | Manager       | Users/Groups |
| `Supervisor`  | Supervisor    | Users/Groups |
| `Staff`       | Staff         | Users/Groups |
| `FieldWorker` | Field Worker  | Users/Groups |
| `Accountant`  | Accountant    | Users/Groups |

### Create SPA App Registration (Next.js)

1. Name: `IMS Frontend`, SPA redirect: `http://localhost:3000/auth/callback`
2. Add API permissions → Select `IMS API` → `Inventory.Read`, `Inventory.Write`
3. Grant admin consent

### Token Configuration

Add optional claims (ID + Access token): `email`, `given_name`, `family_name`

---

## Step 4.2 — Backend Configuration

**`appsettings.json`** — add the `AzureAd` section (actual values in user-secrets/Key Vault):

```jsonc
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_API_CLIENT_ID",
    "Audience": "api://YOUR_API_CLIENT_ID",
  },
}
```

**`Program.cs`** — register authentication:

```csharp
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
```

Pipeline order:

```csharp
app.UseAuthentication();
app.UseMiddleware<TenantMiddleware>();
app.UseMiddleware<UserProvisioningMiddleware>();
app.UseAuthorization();
```

---

## Step 4.3 — Tenant Provider

**File**: `src/IMS.Infrastructure/Services/HttpTenantProvider.cs`

```csharp
public class HttpTenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider
{
    public int TenantId =>
        httpContextAccessor.HttpContext?.Items["TenantId"] is int id ? id : 0;

    public string? UserId =>
        httpContextAccessor.HttpContext?.User
            .FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
}
```

Register: `services.AddHttpContextAccessor(); services.AddScoped<ITenantProvider, HttpTenantProvider>();`

---

## Step 4.4 — User Provisioning Middleware

**File**: `src/IMS.API/Middleware/UserProvisioningMiddleware.cs`

```csharp
public class UserProvisioningMiddleware(RequestDelegate next, ILogger<UserProvisioningMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, ImsDbContext db)
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
                CreatedAt = DateTime.UtcNow
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

    private static int ResolveTenantId(HttpContext context)
    {
        // Priority: JWT claim → Header → Subdomain
        if (int.TryParse(context.User.FindFirst("tenant_id")?.Value, out var c)) return c;
        if (int.TryParse(context.Request.Headers["X-Tenant-Id"].FirstOrDefault(), out var h)) return h;
        return 0;
    }
}
```

---

## Step 4.5 — Base Controller

**File**: `src/IMS.API/Controllers/BaseApiController.cs`

```csharp
[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    protected int GetCurrentTenantId() => HttpContext.Items["TenantId"] is int id ? id : 0;
    protected string? GetCurrentUserOid() => User.FindFirst("oid")?.Value;
    protected User? GetCurrentUser() => HttpContext.Items["CurrentUser"] as User;
}
```

---

## Step 4.6 — Frontend: MSAL.js (Next.js)

```bash
npm install @azure/msal-browser @azure/msal-react
```

**`lib/authConfig.ts`**:

```typescript
import { Configuration } from "@azure/msal-browser";

export const msalConfig: Configuration = {
  auth: {
    clientId: process.env.NEXT_PUBLIC_AZURE_CLIENT_ID!,
    authority: `https://login.microsoftonline.com/${process.env.NEXT_PUBLIC_AZURE_TENANT_ID}`,
    redirectUri:
      process.env.NEXT_PUBLIC_REDIRECT_URI ||
      "http://localhost:3000/auth/callback",
  },
  cache: { cacheLocation: "sessionStorage" },
};

export const loginRequest = {
  scopes: [
    `api://${process.env.NEXT_PUBLIC_AZURE_API_CLIENT_ID}/Inventory.Read`,
    `api://${process.env.NEXT_PUBLIC_AZURE_API_CLIENT_ID}/Inventory.Write`,
  ],
};
```

**`lib/apiClient.ts`** — acquires token silently, attaches as Bearer header to all API calls.

---

## Verification Checklist

- [ ] API rejects requests without valid Bearer token (401)
- [ ] API accepts valid Entra ID tokens
- [ ] JIT provisioning creates user on first login
- [ ] Subsequent logins update `LastLoginAt` only
- [ ] `TenantId` propagated correctly to downstream services
- [ ] Frontend acquires tokens and calls protected endpoints

---

→ [Phase 5 — Authorization & RBAC](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/05-Authorization-RBAC.md)
