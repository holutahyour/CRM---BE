# Phase 4b — Authentication: Azure Entra ID (Identity Only)

> **Goal**: Configure Microsoft Entra ID strictly as an Identity Provider (Authentication), leaving all role and permission management (Authorization) to the application logic.

---

## Step 1 — Azure Portal Configuration

In this strategy, Azure Entra ID is only used to verify **who** the user is. It does not need to know about application-specific roles.

### 1.1 Create API App Registration

1. Navigate to **Microsoft Entra admin center** → **App registrations** → **New registration**.
2. Name: `CRM API`.
3. Supported account types: `Accounts in this organizational directory only (Single tenant)`.
4. Leave Redirect URI blank for now.
5. Click **Register**.

**Note down**:

- **Application (client) ID**
- **Directory (tenant) ID**

### 1.2 Expose an API

1. Go to **Expose an API**.
2. Set the **Application ID URI** (e.g., `api://{client-id}`).
3. Click **Add a scope**.
   - Scope name: `access_as_user`
   - Who can consent: `Admins and users`
   - Display name: `Access CRM API`
   - Description: `Allows the app to access the CRM API as the signed-in user.`

> [!IMPORTANT]
> Do **NOT** create App Roles in the Azure Portal. Role management will be handled internally by the CRM application database.

---

## Step 2 — Backend Configuration (`CRM.API`)

### 2.1 Update `appsettings.json`

Add the `AzureAd` configuration section.

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "your-domain.onmicrosoft.com",
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID",
    "Scopes": "access_as_user"
  }
}
```

### 2.2 Configure Authentication in `Program.cs`

Use Microsoft Identity Web to handle JWT validation.

```csharp
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;

// ...

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Optional: Configure strict audience validation
builder.Services.Configure<JwtBearerOptions>(
    JwtBearerDefaults.AuthenticationScheme,
    options =>
    {
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidAudiences = new[]
        {
            builder.Configuration["AzureAd:ClientId"],
            $"api://{builder.Configuration["AzureAd:ClientId"]}"
        };
    });
```

### 2.3 Middleware Pipeline

Ensure standard ASP.NET Core authentication middleware is used.

```csharp
app.UseAuthentication();
// app.UseMiddleware<UserProvisioningMiddleware>(); // Integrated in Phase 5c
app.UseAuthorization();
```

---

## Step 3 — Verification

- [ ] Use a tool like **Postman** or **Insomnia** to acquire a token from Azure.
- [ ] Call a protected endpoint (marked with `[Authorize]`).
- [ ] Verify the API returns `200 OK` with a valid token and `401 Unauthorized` without one.
- [ ] Inspect the token at [jwt.ms](https://jwt.ms) to ensure the `oid` and `email` claims are present.

---

→ [Phase 5b — Local Application Authorization](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/05b-Auth-Local-Authorization.md)
