# Phase 11 — Postman Testing Guide

> **Goal**: Configure Postman to test the Entra ID protected APIs, including token acquisition and tenant switching.

---

## Step 11.1 — Create Postman Environment

Create a new environment (e.g., `IMS-Dev`) with these variables:

| Variable        | Initial Value                 | Description                                 |
| --------------- | ----------------------------- | ------------------------------------------- |
| `base_url`      | `https://localhost:5001/api`  | API Base URL                                |
| `tenant_id`     | `00000000-0000...`            | Azure AD Directory ID                       |
| `client_id`     | `00000000-0000...`            | Azure AD API Client ID                      |
| `client_secret` | `*****`                       | Client Secret (if using Client Credentials) |
| `scope`         | `api://{client_id}/Admin.All` | The API scope you defined                   |
| `access_token`  | (leave empty)                 | Managed by Postman                          |
| `x_tenant_id`   | `1`                           | Database Tenant ID for testing              |

---

## Step 11.2 — Configure OAuth 2.0 Authentication

For any request (or the whole collection), go to the **Authorization** tab:

1. **Type**: OAuth 2.0
2. **Add auth data to**: Request Headers
3. **Configure New Token**:
   - **Token Name**: `IMS-Token`
   - **Grant Type**: `Authorization Code` (Recommended for user testing)
   - **Callback URL**: `http://localhost:3000/auth/callback` (Must match Azure portal)
   - **Auth URL**: `https://login.microsoftonline.com/{{tenant_id}}/oauth2/v2.0/authorize`
   - **Access Token URL**: `https://login.microsoftonline.com/{{tenant_id}}/oauth2/v2.0/token`
   - **Client ID**: `{{client_id}}`
   - **Scope**: `{{scope}} openid profile email`
4. Click **Get New Access Token**.
5. Log in through the popup.
6. Click **Use Token**.

---

## Step 11.3 — Testing Multi-Tenancy

To test tenant isolation:

1. Add a **Header** to your requests: `X-Tenant-Id: {{x_tenant_id}}`
2. Change `x_tenant_id` to `1`, hit an endpoint (e.g., `GET /api/items`).
3. Change `x_tenant_id` to `2`, hit the same endpoint.
4. Verify that the results change and you only see data for that tenant.

---

## Step 11.4 — Testing RBAC (Roles/Permissions)

1. Create a request for `GET /api/users/me`.
2. Inspect the JSON response to see your `roles` and `permissions`.
3. Try hitting an Admin-only endpoint (e.g., `POST /api/roles`).
4. If your user doesn't have the "Admin" role in Entra ID, you should receive a **403 Forbidden**.

---

## Step 11.5 — Collection Pre-request Script (Optional)

To automatically refresh tokens (Client Credentials only):

```javascript
const tokenHost =
  "https://login.microsoftonline.com/" + pm.environment.get("tenant_id");
const tokenPath = "/oauth2/v2.0/token";

pm.sendRequest(
  {
    url: tokenHost + tokenPath,
    method: "POST",
    header: { "Content-Type": "application/x-www-form-urlencoded" },
    body: {
      mode: "urlencoded",
      urlencoded: [
        { key: "grant_type", value: "client_credentials" },
        { key: "client_id", value: pm.environment.get("client_id") },
        { key: "client_secret", value: pm.environment.get("client_secret") },
        { key: "scope", value: pm.environment.get("scope") },
      ],
    },
  },
  function (err, res) {
    if (res.code === 200) {
      pm.environment.set("access_token", res.json().access_token);
    }
  },
);
```

Then in the Authorization tab, set **Type** to `Bearer Token` and use `{{access_token}}`.

---

## Step 11.6 — Verification Checklist for Testing

- [ ] **401 Unauthorized**: Send request without a token.
- [ ] **200 OK**: Send request with valid token and correct `X-Tenant-Id`.
- [ ] **403 Forbidden**: Send request with valid token but insufficient roles.
- [ ] **Data Isolation**: Switch `X-Tenant-Id` and ensure data is segmented.
- [ ] **JIT Provisioning**: Log in with a new user and verify they appear in the `users` table via DB query.

---

## Next Steps

→ [Master Overview](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/00-Implementation-Overview.md)
