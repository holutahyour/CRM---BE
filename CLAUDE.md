# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Solution Overview

ASP.NET Core 9 CRM backend (`CRM.sln`) with five projects following a clean layered architecture. Authentication is via **Azure AD (Microsoft Entra ID)** JWT bearer tokens. The system supports **multi-tenancy** and **soft deletes** through global EF Core query filters.

## Common Commands

```bash
# Build entire solution
dotnet build CRM.sln

# Run the API (development)
dotnet run --project CRM.API

# Add a migration
dotnet ef migrations add <MigrationName> --project CRM.Data --startup-project CRM.API

# Apply migrations locally
dotnet ef database update --project CRM.Data --startup-project CRM.API

# Seed data only (used in CI/CD production runs)
dotnet run --project CRM.API -- --seed
```

## Project Layer Structure

| Project | Role |
|---|---|
| `CRM.Base` | Foundation: `BaseEntity<T>`, `TenantEntity<T>`, `AuditableEntity`, `IMSSQLRepository<T,I>`, `IMSSQLBaseService<TEntity,TId>`, `Result<T>` |
| `CRM.Domain` | Domain models: entities, DTOs, enums, validators, exceptions |
| `CRM.Data` | EF Core: `ApplicationDbContext`, configurations, migrations, seeds, repository implementations |
| `CRM.Service` | Business logic: service implementations, `AutoMapperConfig` |
| `CRM.API` | Web API: controllers, middleware, `Program.cs` |

Dependencies flow: `API → Service → Data → Domain → Base`

## Architecture Patterns

### Entity Hierarchy

All entities derive from one of:
- `BaseEntity<T>` — adds `Id`, `Code`, `CreatedBy/On`, `LastModifiedBy/On`, `IsDeleted` (soft delete)
- `TenantEntity<T>` — extends `BaseEntity<T>` with `TenantId` (multi-tenant isolation)

The `ApplicationDbContext` applies **global query filters** automatically:
- Entities implementing `ITenantEntity` get filtered by `TenantId` AND `!IsDeleted`
- Entities implementing only `ISoftDelete` get filtered by `!IsDeleted`
- Use `.IgnoreQueryFilters()` when cross-tenant or cross-delete reads are needed (e.g., in authorization and middleware)

### Multi-Tenancy

`TenantMiddleware` resolves `TenantId` from (in priority order): JWT `tenant_id` claim → `X-Tenant-Id` header → subdomain. The resolved `TenantId` is stored in `HttpContext.Items["TenantId"]` and consumed by `TenantProvider` which is injected into `ApplicationDbContext`.

`UserProvisioningMiddleware` runs after authentication to JIT-provision new Entra users into the `Users` table and assign them to a tenant.

### Permission-Based Authorization

Permissions are string codes defined in `CRM.Domain/Constants/Access Control/Permissions.cs`. `PermissionAuthorizationHandler` checks that a user's roles have the required permission via the `User → UserRoles → Role → RolePermissions → Permission` chain. Policies are registered in `Program.cs` and used via `[Authorize(Policy = "...")]`.

### Generic Repository & Service

`IMSSQLRepository<T, I>` (in `CRM.Base`) provides standard CRUD plus paginated `GetAllWithMetaAsync`. Service classes inherit from `MSSQLBaseService<TEntity, TId>` and inject the generic repository. New domain areas follow this same pattern.

### Result Wrapper

All service methods return `Result<T>` which carries `IsSuccess`, `Content`, `ErrorMessage`, and pagination `MetaData`. Controllers unwrap this to determine HTTP response status.

### AutoMapper

All entity↔DTO mappings are registered in `CRM.Service/AutoMapperConfig.cs`. Navigation-property flattening (e.g., `DepartmentName`, role name lists) is done here, not in controllers or services.

### Database Seeding

Static seed data (Modules, Permissions, Menus, Roles, RolePermissions) is applied in `ApplicationDbContext.OnModelCreating` via `HasData`. Operational/runtime seeds run through `Seeder` called from `ServiceProviderExtensions.ApplyMigrationsAndSeed` at development startup, or via the `--seed` CLI flag in production.

## Domain Areas

Controllers, services, entities, and DTOs are organized consistently into these areas:

- **Core** — Countries, Cities, States, Genders, Ethnicities, Parameters
- **Core/Access Control** — Tenants, Users, Roles, Permissions, Menus, Modules
- **Organization** — Departments
- **Inventory** — Categories, Items, Locations, ItemLocations, Batches, InventoryTransactions
- **Orders** — Vendors, PurchaseOrders, SalesOrders
- **Requisitions** — Activities, Incidents, ItemRequests, MonthlyReports, Requisitions, DashboardSummary

## CI/CD

Azure Pipelines (`azure-pipelines.yml`) triggers on `main`. The pipeline: restores → publishes → runs tests → applies EF migrations → seeds data → packages as zip. Production secrets (connection string) come from the `ProductionSecrets` variable group. The pipeline references `Lagetronix.Tabernacle.API` in some steps — this is an older name that may need updating.

---

## Namespace Conventions (Critical — Flat, Not Nested)

Despite physical folder nesting, ALL types in these projects declare a **flat** namespace:

| Physical location | Namespace |
|---|---|
| `CRM.Domain/Entities/**/*.cs` | `CRM.Domain.Entities` |
| `CRM.Domain/DTOs/**/*.cs` | `CRM.Domain.DTOs` |
| `CRM.Domain/Enums/**/*.cs` | `CRM.Domain.Enums` (or `CRM.Domain.Enums.Workflow` for workflow enums) |
| `CRM.Service/Services/Interfaces/**/*.cs` | `CRM.Services.Interfaces` |
| `CRM.Service/Services/Implementations/**/*.cs` | `CRM.Services.Implementations.<Area>` |
| `CRM.Data/Configurations/**/*.cs` | `CRM.Data.Configurations` |

Subfolders are for **file organisation only**, not namespace nesting. Always verify by reading an existing file in the same folder before creating a new one.

**Ambiguous `ApplicationDbContext`:** Both `CRM.Data.ApplicationDbContext` and `CRM.Base.Common.Repositories.ApplicationDbContext` are resolvable in the Service project. Always use the fully qualified type `CRM.Data.ApplicationDbContext` in service class field declarations and constructor parameters to avoid compiler ambiguity.

---

## Testing (CRM.Tests)

Test project: `CRM - BE/CRM.Tests/CRM.Tests.csproj`
Stack: **xUnit 2.9, Moq 4.20, FluentAssertions 6.12, EF Core InMemory 9.0**

### TestDbContext Helper

```csharp
// CRM.Tests/Helpers/TestDbContext.cs
var db = TestDbContext.Create(tenantId, userId);  // both optional Guid?
```

- Creates an isolated **InMemory** `ApplicationDbContext` with a unique DB name per call.
- Mocks `ITenantProvider` (namespace: `CRM.Services.Services.Interfaces.Common`).
- `userId` is a `string?` (parses to Guid or defaults to new Guid if null).

### Key Testing Patterns

**Always use `.IgnoreQueryFilters()`** when seeding and querying in tests. The global tenant + soft-delete filters will block reads if the seeded `TenantId` doesn't match the mocked provider's `TenantId`. In service implementations that need cross-filter reads (workflow, approval), use `.IgnoreQueryFilters()` with an explicit tenantId predicate to preserve isolation:
```csharp
.IgnoreQueryFilters()
.Where(t => t.TenantId == tenantId && t.IsActive)
```

**Clear the ChangeTracker before re-reading entities in tests:**
```csharp
db.ChangeTracker.Clear();
var updated = await db.Requisitions.FindAsync(req.Id);
```

**TenantId is auto-assigned by `SaveChangesAsync`** — do NOT set it manually in service implementations. The `ApplicationDbContext` reads it from `ITenantProvider` and applies it to new entities before saving. Setting it manually would conflict with the InMemory test provider.

**`Xunit` namespace must be explicitly imported** in test files — it is not in global usings.

---

## Approval Workflow Domain Area

Added in the `ai_development` branch. Tables: `wf_workflow_templates`, `wf_workflow_steps`, `wf_approval_records`.

### Architecture

- **Shared tables with `WorkflowType` discriminator** — one template/step/record schema serves both Requisitions and Item Requests.
- `WorkflowType` enum: `Requisition = 1`, `ItemRequest = 2`
- `ApprovalStatus` enum: `Pending = 1`, `Approved = 2`, `Rejected = 3`

### Authorization Check in ApprovalService

For each approval step, the service validates the acting user in this order:
1. If `WorkflowStep.UserId` is set → user must match exactly (user override, bypasses role check)
2. Otherwise → user must have `WorkflowStep.RoleId` in their `UserRoles`

Throws `UnauthorizedAccessException` if neither condition is met. Throws `InvalidOperationException("No active workflow template...")` if no template exists for the type.

### Key Services

| Interface | Implementation | Responsibility |
|---|---|---|
| `IWorkflowService` | `WorkflowService` | CRUD for workflow templates and steps |
| `IApprovalService` | `ApprovalService` | Approve/reject entities, record history |
| `IBlobStorageService` | `BlobStorageService` | Upload files to Azure Blob Storage |

All three are registered as `AddScoped` in `CRM.Service/DependencyInjection.cs`.

### Permissions

Permission codes use **dot-notation** (not PascalCase constants):
- `"workflows.templates.view"` → `Permissions.WorkflowTemplatesView`
- `"workflows.templates.manage"` → `Permissions.WorkflowTemplatesManage`

The `All` list is auto-generated via reflection — adding a `public const string` to `Permissions.cs` automatically includes it in `PermissionSeedData` and `RolePermissionSeedData`.

### EF Configuration

`ApplicationDbContext.OnModelCreating` uses `modelBuilder.ApplyConfigurationsFromAssembly(...)` — **do not add explicit `ApplyConfiguration()` calls**. Any class implementing `IEntityTypeConfiguration<T>` in `CRM.Data` is auto-discovered. New workflow config files go in `CRM.Data/Configurations/` (flat, no Workflow subfolder) with namespace `CRM.Data.Configurations`.

### File Uploads (Requisitions)

`BlobStorageService` requires `AzureBlob:ConnectionString` and `AzureBlob:ContainerName` in `appsettings.json`. The connection string ships with `REPLACE_ME` placeholders — configure real values via environment variables, user secrets, or Key Vault before using file upload in production.

`RequisitionService.CreateWithFileAsync` uploads the stream then sets `FileUrl` / `FileOriginalName` on the request before calling `CreateAsync`. The multipart controller endpoint reads `IFormFile? file` and passes its stream.

### ItemRequest Side-Effects

The original `ItemRequestService.ApproveAsync` had inline side-effects: **inventory quantity deduction** and **activity log writes**. After delegating to `IApprovalService`, these are no longer executed. If those side-effects need to be preserved, they must be re-added to `ApprovalService.ApproveAsync` for `WorkflowType.ItemRequest` or triggered separately in the controller.
