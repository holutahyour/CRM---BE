# CRM backend (CRM---BE)

ASP.NET Core Web API behind the Eupepsia / Soilless Farm Lab CRM. It serves the Next.js frontend
([CRM--FE/README.md](../CRM--FE/README.md), which describes every page from the user's side).
This file describes what the API does: how requests are authenticated and scoped to a tenant, the
rules each domain area enforces, the full endpoint list, and the known gaps.

For code conventions (flat namespaces, test helpers, the approval-workflow internals) see
[CLAUDE.md](CLAUDE.md). For where the seeded data came from and every value that was inferred,
see [docs/IMPORT-NOTES.md](docs/IMPORT-NOTES.md).

---

## 1. Running it

### Projects

| Project | Role |
|---|---|
| `CRM.Base` | Base entities (`BaseEntity`, `TenantEntity`), generic repository/service/controller, `Result<T>` |
| `CRM.Domain` | Entities, DTOs, enums, constants (permissions, processing categories), FluentValidation validators |
| `CRM.Data` | `ApplicationDbContext`, EF configurations, migrations, seeders and seed JSON |
| `CRM.Service` | Business logic, AutoMapper profile, DI registration |
| `CRM.API` | Controllers, middleware, `Program.cs` |
| `CRM.Tests` | xUnit tests (EF InMemory and in-memory SQLite) |

Dependencies flow `API → Service → Data → Domain → Base`.

### Commands

```bash
dotnet build CRM.sln
dotnet test CRM.Tests
dotnet run --project CRM.API            # http://localhost:5297, https://localhost:7290
dotnet run --project CRM.API -- --seed  # run the seeders and exit (used by CI)
```

Swagger UI is at `/swagger` (always in Development; elsewhere when `Swagger:Enabled` is true, which
`appsettings.json` sets). Health check: `GET /health`.

### Configuration

| Key | Meaning |
|---|---|
| `DatabaseProvider` | `SqlServer` (default when unset) or `Sqlite` |
| `ConnectionStrings:DefaultConnection` | Must match the provider. A mismatch fails at startup with a message naming both values |
| `AzureAd:*` | Entra ID tenant, API client id and accepted audiences (`<ClientId>` and `api://<ClientId>`) |
| `Auth:SuperAdminEmails` | Accounts that are granted `SUPER_ADMIN` on every request (see §2.2) |
| `AzureBlob:ConnectionString`, `AzureBlob:ContainerName` | Storage for requisition attachments |
| `Swagger:Enabled` | Turns Swagger on outside Development |

> **Warning: local runs hit the live database by default.** `appsettings.json` has no
> `DatabaseProvider` and its `DefaultConnection` points at the hosted SQL Server. Development
> startup applies migrations and runs the seeders (below), so a plain `dotnet run` migrates and seeds
> that database. To work locally, override both:
>
> ```bash
> DatabaseProvider=Sqlite ConnectionStrings__DefaultConnection="Data Source=crm_dev.db;Foreign Keys=False" dotnet run --project CRM.API
> ```
>
> An existing `crm_dev.db` never gains new tables. Delete it to pick up schema changes.

### Startup behaviour

- **Development:** every start applies pending migrations, then runs `Seeder.Intialize()`, which
  runs every seeder in §4.8.
- **`--seed`:** runs `Seeder.Intialize()` against the configured database and exits without serving.
- **Other environments:** no automatic migration. The Azure pipeline runs `dotnet ef database update`
  before seeding.

### Migrations

Scaffold with the pinned tool and the SQL Server provider. Scaffolding under SQLite produces the
wrong column types.

```bash
DatabaseProvider=SqlServer ASPNETCORE_ENVIRONMENT=Production \
  dotnet tool run dotnet-ef migrations add <Name> --project CRM.Data --startup-project CRM.API
```

### Deployment

- `azure-pipelines.yml` (on `main`): restore, publish, test, apply migrations, seed, zip artifact.
  The connection string comes from the `ProductionSecrets` variable group.
- `.github/workflows/soilless-farmlab-crm.yml`: build, test, publish to the Azure Web App
  `soilless-farmlab-crm`.
- `Dockerfile`: .NET 10 SDK/runtime images, listens on port 8080, `/app/data` for a SQLite volume.
- The site the frontend uses today is `crm-app.runasp.net` (MonsterASP, Web Deploy), not the Azure
  app named in the GitHub workflow.

---

## 2. What happens to a request

Middleware runs in this order: CORS → HTTPS redirect → request logging → **authentication** →
**tenant resolution** → **user provisioning** → **authorization** → controller.

### 2.1 Authentication

Callers send an Entra ID access token (`Authorization: Bearer …`). Tokens are validated against
`AzureAd` config, and the audience must be the API's client id or `api://<client id>`. The user is
identified by the `oid` (object id) claim.

Authentication is only *enforced* where an endpoint has `[Authorize]`. Many endpoints don't. See
§5 and §6.

### 2.2 First sign-in (JIT provisioning)

On any authenticated request, `UserProvisioningMiddleware` looks the caller up by `oid`:

- **Unknown user:** a `User` row is created from the token claims (email, names, phone) with
  `Status = NotOnboarded`, `Onboarded = false`, `IsActive = true`. The tenant is taken from the
  `tenant_id` claim, else the `X-Tenant-Id` header, else the `SYSTEM` tenant, else the first tenant.
  - The **very first user in the database** also gets the `ADMIN` role, so the system is never
    locked out.
  - Everyone else gets **no role** and appears in User Management as *Not Onboarded* until an
    administrator assigns roles and onboards them.
  - Parallel first requests that race to insert the same user are handled: the loser reuses the
    winner's row.
- **Known user:** `LastLoginAt` is updated. A user with an empty `TenantId` is moved to `SYSTEM`.
- **Super admins:** if the user's email is in `Auth:SuperAdminEmails`, the `SUPER_ADMIN` role is
  added if it's missing. This takes effect on the same request, no re-login.

The resolved user and tenant are stored in `HttpContext.Items` (`CurrentUser`, `TenantId`) for
services to read.

### 2.3 Tenant resolution

Every tenant-owned table is filtered to one tenant. The tenant comes from, in order:
`HttpContext.Items["TenantId"]` (set by the middleware) → the `tenant_id` JWT claim →
the **`X-Tenant-Id` request header** → `Guid.Empty`.

`TenantProvider` reads the header **even on anonymous requests**, which is the root of the security
gap in §6.

### 2.4 Authorization

Policies map to permission codes (`CRM.Domain/Constants/Access Control/Permissions.cs`, 55 codes).
`PermissionAuthorizationHandler` reads the database on every check and passes when any of the
user's roles holds the permission (`User → UserRoles → Role → RolePermissions → Permission`,
ignoring soft-deleted links). Role changes therefore apply immediately.

| Policy | Permission |
|---|---|
| `AdminOnly` | users.manage |
| `SuperAdminOnly` | system.manage |
| `AuditView` | audit.view |
| `ModulesView` / `ModulesManage` | modules.view / modules.manage |
| `WorkflowTemplatesView` / `WorkflowTemplatesManage` | workflow templates view / manage |
| `OperationsView` / `OperationsManage` | operations.view / operations.manage |
| `ManagerOrAbove`, `SupervisorOrAbove`, `InventoryRead`, `InventoryWrite`, `PurchaseOrderApprove`, `ReportsView`, `RolesManage`, `MenusManage` | Registered but not used by any endpoint yet |

Seeded roles (all `IsSystem`) in the `SYSTEM` tenant: `SUPER_ADMIN`, `ADMIN`, `MANAGER`,
`MANAGER_ACCESS`, `DEPT_USER`, `ARTISAN`, `STAFF`.

### 2.5 Persistence rules applied to every write

- **Tenant stamping:** new tenant-owned rows get the current tenant when they don't carry one.
- **Soft delete:** deletes set `IsDeleted`. Deleted rows disappear from every query (global filter).
- **Who/when:** `CreatedBy/On` and `LastModifiedBy/On` are set from the caller's `oid` (or `SYSTEM`).
- **Audit log:** the generic create/update/delete in `MSSQLBaseService` writes an `AuditLog` row
  (action, entity name, user, IP, tenant, old and new values as JSON). Services that bypass the
  generic methods (stock-card postings, approvals, seeders) do not write audit rows.
- **Record codes:** the generic create assigns a random 10-character `Code`.

---

## 3. API conventions

- Base path `api/v1/…`. Controllers without an explicit route use their class name
  (`/api/v1/items`, `/api/v1/requisitions`). The seed endpoints sit at the root (`/seed-…`).
- **Response envelope** (`Result<T>`): `isSuccess`, `content`, `message`, `errorMessage`,
  `dataCount`, pagination `metaData` (`total`, `from`, `to`, `perPage`, `lastPage`, links),
  `requestTime`, `responseTime`. A failed result returns **HTTP 400** with the
  same envelope, so clients should read the message from the response body.
- **List query parameters** (generic `GET` on every CRUD controller): `search` (all properties),
  `filter` (e.g. `Department=Accounting`), `page` (default 1), `pageSize` (default **100**),
  `select`, `orderBy`, `orderDirection` (`Asc`/`Desc`).
- Enums are serialised as **strings**. Date-only fields use `DateOnly` (`"2026-09-29"`).
- Validation: validators exist in `CRM.Domain/Validators`, but FluentValidation is not wired into
  MVC. Only services that call their validators explicitly (Operations) reject bad input. Elsewhere
  the database constraints are the only check.

---

## 4. Domain behaviour

### 4.1 Users, roles, menus, modules, tenants

- **`GET /users/me`** returns the caller's profile, roles and permissions. The frontend builds its
  permission checks from this.
- **Onboarding:** `PATCH /users/{id}/onboard` sets `Status = Onboarded`. Roles are added and removed
  with `POST /users/{id}/roles` and `DELETE /users/{id}/roles/{roleId}`.
- **Roles:** CRUD plus `GET /roles/names` for any signed-in user (drop-downs). `IsSystem` is stored
  but not enforced: system roles can be edited and deleted.
- **Menus:** `GET /menus/my-menus` builds the sidebar tree. A menu is shown when it is active, its
  module (if any) is active for the tenant, and it has no permission requirement or the user holds
  one of its permissions. `ADMIN` users are treated as holding every permission. Children are
  nested under parents and ordered by `Position`.
- **Modules:** a global catalogue (`GET /modules`) plus per-tenant activation
  (`GET /modules/tenant`, `POST /modules/toggle`). Enabling a module the tenant never had creates the
  activation row. Disabling one it never had does nothing. Only super admins can change the catalogue.
- **Tenants:** `POST /tenants/onboarding` creates a tenant (subscription `Trial`) with its own
  `ADMIN`, `MANAGER` and `STAFF` roles. The request's admin email is accepted but not used: no user
  or invitation is created.
- **Audit logs:** `GET /auditlogs` (filters: entity, action, user, date range; page size 1–500,
  default 50; newest first) and `GET /auditlogs/export` (CSV, capped row count). The `allTenants`
  flag is honoured only for callers with `system.manage`; everyone else sees their own tenant.

### 4.2 Approval workflow (requisitions and item requests)

Both use one template/step/record schema, told apart by `WorkflowType` (`Requisition = 1`,
`ItemRequest = 2`).

- A tenant has one active **template** per type, with ordered **steps**. Each step names a role, or
  a specific user who overrides the role.
- A new requisition or item request starts at step 1 as `Pending`.
- **Approve** (`PUT /{entity}/{id}/approve`): the caller must be the step's user, or hold the
  step's role, or the request fails. The approval is recorded. At the last step the entity becomes
  `Approved`. Otherwise it moves to the next step and stays `Pending`.
- **Reject** (`PUT /{entity}/{id}/reject`, body `{ reason }`): recorded with the reason. The entity
  becomes `Rejected` at any step.
- **History** (`GET /{entity}/{id}/approval-history`): every step with its approver and outcome,
  plus the current step.
- With no active template for the type, approve and reject fail with "No active workflow template…".
- Templates: `GET /workflows`, `GET /workflows/{type}`, `POST /workflows`,
  `PUT /workflows/{id}/steps`.

### 4.3 Requisitions, item requests, incidents, monthly reports

- **Requisitions** are created as `multipart/form-data`. An optional file is uploaded to Azure Blob
  Storage and its URL stored on the record. The submitter is the current user. Creation also writes a
  "New Requisition … submitted" **Activity** for the dashboard feed.
- **Item requests** record the requester and write an Activity. **Approving one does not deduct
  stock**: inventory is unchanged.
- **Incidents** record the reporter and write an Activity. `PUT /incidents/{id}/in-progress` and
  `PUT /incidents/{id}/resolve` (with a resolution) set the status. There is no state guard: a
  resolved incident can be moved back to in progress.
- **Monthly reports** record the submitter. No delete endpoint.
- **Dashboard** (`GET /dashboardsummaries`): counts pending and approved requisitions, pending item
  requests and open incidents. **`LowStock` is always 5 and `MonthlyGoalsAchieved` always 85**
  (hard-coded).

### 4.4 Inventory

Items, categories, locations, item locations, batches, vendors and inventory transactions have
generic CRUD plus bulk `POST …/import`.

- **Low stock** (`GET /items/low-stock`): items with a `MinStockLevel` whose `QuantityOnHand` is at
  or below it.
- **Creating a transaction** through `POST /inventorytransaction` adjusts the item's
  `QuantityOnHand`. Purchase, TransferIn, Return, Production and Adjustment add; everything else
  subtracts. Updating or deleting a transaction does **not** reverse its effect.
- `RecordTransactionAsync` (used internally) stamps the transaction with *now*, not a chosen date, and
  clamps stock at zero rather than rejecting an over-issue. The Operations stock cards avoid it for
  that reason.

### 4.5 Operations → Processing

This area captures the *Batch Production Scheduling* workbook. The design is in
[../CRM--FE/docs/superpowers/specs/2026-09-28-operations-processing-design.md](../CRM--FE/docs/superpowers/specs/2026-09-28-operations-processing-design.md).
All routes require `OperationsView`. Writes require `OperationsManage`.

| Resource | Route | Notes |
|---|---|---|
| Products | `/operations/products` | Name required and unique per tenant ("…already exists") |
| Order requests | `/operations/order-requests` | Request date, customer name and products required; due date ≥ start date |
| Production batches | `/operations/batches` | Product names required; end date ≥ start date; on-time delivery 0–100; optional links to an order request and a product must exist |
| Yield entries | `/operations/yield-entries` | Date and produce item required; the produce item and optional batch must exist; all weights ≥ 0 |
| Stock cards | `/operations/stock-cards` | Ledger view over Inventory, below |

Every record has a seven-value `Status`: `NotStarted`, `InProgress`, `Complete`, `OnHold`,
`Overdue`, `NeedsReview`, `NeedsUpdate`.

**Yield maths** (computed on each response, not stored):

- `inputKg`: the input weight, or the input quantity when its unit is kg.
- `outputKg`: the last stage recorded (second grind, then grind, dehydrated, cut).
- `yieldPercent` = output ÷ input × 100, and `wastePercent` = waste ÷ input × 100. Both are null when
  input is missing or zero.

**Stock cards** are the Inventory items in the three processing categories: *Processing – Raw
Produce*, *Processing – Ingredients*, *Processing – Packaging & Supplies*. They share stock with the
Inventory module; there is no second stock figure.

- `GET /operations/stock-cards`: the items, with category, location, unit and quantity on hand.
- `GET /operations/stock-cards/{itemId}`: a ledger of date, opening, received, issued, closing and
  "where required". The opening balance is worked backwards from the current quantity on hand, so
  the last closing always equals it.
- `POST …/{itemId}/receive` and `POST …/{itemId}/issue` with `{ date, quantity, whereRequired? }`:
  post a Purchase or Consumption transaction on the entered date and update quantity on hand.
  - Quantity must be above zero.
  - Issuing more than is on hand is rejected ("Cannot issue X unit of Name: only Y unit on hand.").
  - Items outside the processing categories are rejected.
- No automatic links: a batch or yield entry does not move stock.

### 4.6 Sales

`/sales/daily-production`, `/sales/feed-costs`, `/sales/records` and `/sales/stock` support list,
get, create and delete only. There is no update.

### 4.7 Reference data

Countries (read-only), cities, states (no delete), genders, ethnicities, parameter values and
departments: generic CRUD plus import.

### 4.8 Seeding

`Seeder.Intialize()` runs at Development startup and with `--seed`, and calls every seeder below.
The four seed endpoints run the data seeders individually and return a summary. Every seeder is idempotent: re-running adds only
what is missing.

| Seeder | Endpoint | Seeds |
|---|---|---|
| Base (`Seeder`) | (startup, `--seed`) | `SYSTEM` tenant, the seven roles and their permissions, then the four seeders below |
| `OperationalDataSeeder` | `POST /seed-operational-data` | Eupepsia departments (inferred from the staff list) |
| `InventoryDataSeeder` | `POST /seed-inventory-data` | Inventory categories, locations, items and their opening transactions |
| `WorkflowDataSeeder` | `POST /seed-workflow-data` | Requisition and item-request templates: Manager, then Administrator |
| `ProcessingDataSeeder` | `POST /seed-processing-data` | Main Store and Packaging Store, the three processing categories, 120 materials (existing items with the same name are reused and recategorised; new ones get `PROC-0001…` SKUs) and 16 products |

Modules, permissions and menus are static `HasData` seed in the migrations. Seed JSON lives in
`CRM.Data/Seeds/Data/Eupepsia-Seed-Data/` and is regenerated by the scripts in `tools/import/`.
Seeders use the `SYSTEM` tenant and read with `IgnoreQueryFilters`. That tenant's id is
database-generated, not `Guid.Empty`.

---

## 5. Endpoint index

All paths are under `/api/v1` except the seeders. **Auth** shows what is actually enforced today:
*none* means an anonymous caller is accepted.

Every generic CRUD controller (marked †) also exposes `GET /` (list) and `GET /{id}`. Those inherited
reads carry no `[Authorize]`.

| Area | Endpoints | Auth |
|---|---|---|
| Users | `GET /users`, `GET /users/{id}`, `PUT`, `DELETE`, `POST /users/import`, `PATCH /users/{id}/onboard`, `POST /users/{id}/roles`, `DELETE /users/{id}/roles/{roleId}` | AdminOnly |
| | `GET /users/me` | signed in |
| | `POST /users` | **none** |
| Roles | `GET /roles`, `GET /roles/{id}`, `PUT`, `DELETE`, `POST /roles/import` | AdminOnly |
| | `GET /roles/names` | signed in |
| | `POST /roles` | **none** |
| Permissions | `GET /permissions` | AdminOnly |
| | `GET /permissions/{id}` † | **none** |
| Menus | `GET /menus`, `GET /menus/{id}`, `POST`, `PUT`, `DELETE` | AdminOnly |
| | `GET /menus/my-menus` | `[AllowAnonymous]` (needs a signed-in user to return anything) |
| Modules | `GET /modules`, `GET /modules/tenant` | ModulesView |
| | `POST /modules/toggle` | ModulesManage |
| | `POST /modules`, `PUT /modules/{id}`, `DELETE /modules/{id}` | SuperAdminOnly |
| Tenants † | `GET /tenants`, `GET /tenants/{id}`, `POST /tenants/onboarding` | **none** |
| Audit logs | `GET /auditlogs`, `GET /auditlogs/export` | AuditView |
| Workflows | `GET /workflows`, `GET /workflows/{type}` | WorkflowTemplatesView |
| | `POST /workflows`, `PUT /workflows/{id}/steps` | WorkflowTemplatesManage |
| Operations † | `/operations/products`, `/order-requests`, `/batches`, `/yield-entries`: GET, GET {id} | OperationsView |
| | same resources: POST, PUT {id}, DELETE {id} | OperationsManage |
| | `GET /operations/stock-cards`, `GET /operations/stock-cards/{itemId}` | OperationsView |
| | `POST /operations/stock-cards/{itemId}/receive`, `/issue` | OperationsManage |
| Requisitions † | `POST` (multipart), `PUT`, `POST /import`, `PUT /{id}/approve`, `PUT /{id}/reject`, `GET /{id}/approval-history` | **none** (approve/reject still check the workflow step's role) |
| Item requests † | `POST`, `PUT`, `POST /import`, `PUT /{id}/approve`, `PUT /{id}/reject`, `GET /{id}/approval-history` | **none** (as above) |
| Incidents † | `POST`, `PUT`, `DELETE`, `POST /import`, `PUT /{id}/in-progress`, `PUT /{id}/resolve` | **none** |
| Monthly reports † | `POST`, `PUT`, `POST /import` | **none** |
| Activities † | `POST`, `PUT`, `DELETE`, `POST /import` | **none** |
| Dashboard † | `GET /dashboardsummaries` | **none** |
| Inventory † | `items` (+ `GET /items/low-stock`), `categories`, `locations`, `itemlocations`, `batches`, `inventorytransaction`: `POST`, `PUT`, `DELETE`, `POST /import` | **none** |
| Vendors † | `POST`, `PUT /{id}`, `DELETE /{id}` | **none** |
| Departments † | `POST`, `PUT`, `DELETE`, `POST /import` | **none** |
| Sales † | `/sales/daily-production`, `/sales/feed-costs`, `/sales/records`, `/sales/stock`: `POST`, `DELETE /{id}` | **none** |
| Reference † | `countries` (read only), `cities`, `states` (no delete), `genders`, `ethnicities`, `paramatervalues`: `POST`, `PUT`, `DELETE`, `POST /import` | **none** |
| Seeders | `POST /seed-operational-data`, `/seed-inventory-data`, `/seed-workflow-data`, `/seed-processing-data` (no `/api/v1` prefix) | **none** |
| Health | `GET /health` | none |

Several generic `PUT`/`DELETE` actions take the id as a query parameter (`PUT /departments?id=…`)
rather than in the path. Check Swagger for the exact shape of each.

---

## 6. Known gaps

### Security (fix before exposing more data)

1. **Most endpoints accept anonymous callers.** Only the controllers listed with a policy in §5 have
   `[Authorize]`. Everything marked *none* can be called without a token.
2. **Anonymous callers can choose a tenant.** `TenantProvider` falls back to the `X-Tenant-Id` header
   without checking that the caller is signed in or belongs to that tenant. `GET /tenants` lists
   tenant ids anonymously. Together, anyone who can reach the API can read and write any tenant's
   inventory, requisitions, incidents, sales and departments.
3. **Seed endpoints and tenant onboarding are anonymous.** They write data.
4. **`POST /users` and `POST /roles` are anonymous.** Assigning roles is protected; creating
   records is not.
5. **CORS allows any origin** (`WithOrigins("*")`).
6. **Secrets in source control.** `appsettings.json` holds the hosted database's connection string,
   including its password. Move it to user secrets or environment settings and rotate the password.

A reasonable first step: put `[Authorize]` on `MSSQLBaseController` and `SeedersController`, and
stop reading `X-Tenant-Id` for anonymous requests. Check the frontend still sends a token on every
call before doing this.

### Behaviour

- Item-request approval doesn't deduct stock. Updating or deleting an inventory transaction doesn't
  reverse its stock change.
- Dashboard `LowStock` (5) and `MonthlyGoalsAchieved` (85) are hard-coded.
- `IsSystem` roles can be edited and deleted.
- Incidents have no status transition rules.
- Stock-card postings have no concurrency token. Two simultaneous issues can both pass the
  on-hand check.
- FluentValidation isn't wired into MVC, so most areas accept any shape the database allows.

### Frontend calls with no backend endpoint

The frontend calls these, but the API has no matching route yet. See the frontend README's
"Known gaps":

- facility, vehicle and machine usage logs;
- user activate/deactivate and unauthorized-access logs;
- ERP settings;
- notifications (no SignalR hub);
- the configuration namespaces.

The frontend's department edit URL doesn't match the backend's `PUT /departments?id=…` shape.

### Documentation

`docs/CODEBASE_DOCUMENTATION.md` describes an older system (Educ8eHost), not this API. The IMS
design documents in `docs/` are background research.

---

## 7. Tests

```bash
dotnet test CRM.Tests
```

Tests cover the approval workflow, workflow templates and seeding, audit-log filtering and export,
my-menus filtering, module toggling, requisition creation, blob upload, sales, the Operations
services (validation, link checks, yield maths), stock cards (ledger, over-issue, category scope),
the processing seeder (idempotency, item reuse) and global-role reconciliation.

Use `TestDbContext.Create(...)` (EF InMemory) for fast unit tests and `SqliteTestDb.Create(...)`
when relational behaviour matters (unique indexes, SQL translation). See [CLAUDE.md](CLAUDE.md) for
the pitfalls: query filters, `ChangeTracker.Clear()`, and seeding the principals of required includes.
