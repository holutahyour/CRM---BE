# Phase 1 — Project Setup & Foundation

> **Goal**: Create the .NET solution with Clean Architecture structure, install all NuGet packages, configure base classes, and set up the development environment.

---

## Step 1.1 — Create the Solution & Projects

```bash
# Create solution directory
mkdir IMS && cd IMS

# Create solution
dotnet new sln -n IMS

# Create projects
dotnet new classlib -n IMS.Domain -o src/IMS.Domain
dotnet new classlib -n IMS.Application -o src/IMS.Application
dotnet new classlib -n IMS.Infrastructure -o src/IMS.Infrastructure
dotnet new webapi -n IMS.API -o src/IMS.API

# Create test projects
dotnet new xunit -n IMS.UnitTests -o tests/IMS.UnitTests
dotnet new xunit -n IMS.IntegrationTests -o tests/IMS.IntegrationTests

# Add projects to solution
dotnet sln add src/IMS.Domain/IMS.Domain.csproj
dotnet sln add src/IMS.Application/IMS.Application.csproj
dotnet sln add src/IMS.Infrastructure/IMS.Infrastructure.csproj
dotnet sln add src/IMS.API/IMS.API.csproj
dotnet sln add tests/IMS.UnitTests/IMS.UnitTests.csproj
dotnet sln add tests/IMS.IntegrationTests/IMS.IntegrationTests.csproj
```

## Step 1.2 — Set Up Project References

```bash
# Application depends on Domain
dotnet add src/IMS.Application reference src/IMS.Domain

# Infrastructure depends on Application (and transitively Domain)
dotnet add src/IMS.Infrastructure reference src/IMS.Application

# API depends on Infrastructure (and transitively Application + Domain)
dotnet add src/IMS.API reference src/IMS.Infrastructure

# Tests depend on relevant projects
dotnet add tests/IMS.UnitTests reference src/IMS.Application
dotnet add tests/IMS.UnitTests reference src/IMS.Infrastructure
dotnet add tests/IMS.IntegrationTests reference src/IMS.API
```

### Dependency Rules

```
IMS.Domain         → (no dependencies, pure C#)
IMS.Application    → IMS.Domain
IMS.Infrastructure → IMS.Application
IMS.API            → IMS.Infrastructure
```

> **Critical rule**: `IMS.Domain` must NEVER reference any other project. It contains only POCOs, enums, and interfaces.

## Step 1.3 — Install NuGet Packages

### IMS.Domain (no packages needed — pure C#)

### IMS.Application

```bash
cd src/IMS.Application
dotnet add package FluentValidation --version 11.*
dotnet add package MediatR --version 12.*
dotnet add package Mapster --version 7.*
```

### IMS.Infrastructure

```bash
cd src/IMS.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.*
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.*
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.*
dotnet add package StackExchange.Redis --version 2.*
dotnet add package Azure.Storage.Blobs --version 12.*
dotnet add package Serilog.AspNetCore --version 8.*
dotnet add package Serilog.Sinks.ApplicationInsights --version 4.*
```

### IMS.API

```bash
cd src/IMS.API
dotnet add package Microsoft.Identity.Web --version 3.*
dotnet add package Swashbuckle.AspNetCore --version 6.*
dotnet add package Microsoft.ApplicationInsights.AspNetCore --version 2.*
dotnet add package AspNetCoreRateLimit --version 5.*
dotnet add package Microsoft.AspNetCore.Diagnostics.HealthChecks
```

### Tests

```bash
cd tests/IMS.UnitTests
dotnet add package Moq --version 4.*
dotnet add package FluentAssertions --version 6.*
dotnet add package Bogus --version 35.*

cd tests/IMS.IntegrationTests
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 8.*
dotnet add package Testcontainers.MsSql --version 3.*
```

## Step 1.4 — Create Folder Structure

```
src/IMS.Domain/
├── Entities/
│   ├── Common/          # BaseEntity, TenantEntity
│   ├── Core/            # User, Role, Permission, Menu, Module, Tenant
│   ├── Inventory/       # Item, Category, Batch, ItemLocation, Location
│   └── Orders/          # PurchaseOrder, SalesOrder, Supplier
├── Enums/               # All application enums
├── Constants/           # PermissionConstants, ModuleConstants
├── Events/              # Domain events
└── Exceptions/          # Domain exceptions

src/IMS.Application/
├── Common/
│   ├── Interfaces/      # IRepository, IUnitOfWork, ICacheService
│   ├── Models/          # ApiResponse, PagedResponse, PagedQuery
│   ├── Behaviors/       # MediatR pipeline behaviors (validation, logging)
│   └── Mappings/        # Mapster mapping configurations
├── Features/
│   ├── Users/           # Commands, Queries, DTOs, Validators
│   ├── Roles/
│   ├── Menus/
│   ├── Items/
│   ├── Categories/
│   ├── Locations/
│   ├── Batches/
│   ├── Suppliers/
│   ├── PurchaseOrders/
│   ├── SalesOrders/
│   └── Reports/
└── Services/            # Application service interfaces

src/IMS.Infrastructure/
├── Data/
│   ├── Context/         # ImsDbContext
│   ├── Configurations/  # EF entity type configurations
│   ├── Migrations/      # EF migrations
│   ├── Interceptors/    # Audit interceptor, soft-delete interceptor
│   └── Seed/            # Seed data classes
├── Repositories/        # Repository implementations
├── Services/            # External service implementations (Blob, Redis, etc.)
└── Extensions/          # DI registration extension methods

src/IMS.API/
├── Controllers/         # API controllers
├── Middleware/           # Tenant, UserProvisioning, ExceptionHandler
├── Filters/             # Action filters
├── Extensions/          # Service collection extensions
└── Program.cs           # Application entry point
```

## Step 1.5 — Create Base Entity Classes

**File**: `src/IMS.Domain/Entities/Common/BaseEntity.cs`

```csharp
namespace IMS.Domain.Entities.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
```

**File**: `src/IMS.Domain/Entities/Common/TenantEntity.cs`

```csharp
namespace IMS.Domain.Entities.Common;

public abstract class TenantEntity : BaseEntity
{
    public int TenantId { get; set; }
}
```

## Step 1.6 — Create Common Response Models

**File**: `src/IMS.Application/Common/Models/ApiResponse.cs`

```csharp
namespace IMS.Application.Common.Models;

public record ApiResponse<T>(
    bool Success,
    T? Data = default,
    string? Message = null,
    IEnumerable<string>? Errors = null
);

public record PagedResponse<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}

public record PagedQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? SortBy = null,
    bool SortDescending = false
);
```

## Step 1.7 — Configure `appsettings.json`

**File**: `src/IMS.API/appsettings.json`

```jsonc
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "<your-tenant-id>",
    "ClientId": "<your-api-client-id>",
    "Audience": "api://<your-api-client-id>",
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:<server>.database.windows.net;Database=IMS;Authentication=Active Directory Default;",
    "Redis": "<redis-connection-string>",
  },
  "BlobStorage": {
    "ConnectionString": "<blob-connection-string>",
    "ContainerName": "ims-assets",
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning",
    },
  },
  "ApplicationInsights": {
    "ConnectionString": "<app-insights-connection-string>",
  },
}
```

## Step 1.8 — Configure `Program.cs` (skeleton)

**File**: `src/IMS.API/Program.cs`

```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// === Services registered in Phases 3–6 ===
// builder.Services.AddInfrastructure(builder.Configuration);  // Phase 3
// builder.Services.AddAuthentication(...)                     // Phase 4
// builder.Services.AddAuthorization(...)                      // Phase 5

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

// === Middleware registered in Phases 4–6 ===
// app.UseAuthentication();                                    // Phase 4
// app.UseMiddleware<TenantMiddleware>();                      // Phase 6
// app.UseMiddleware<UserProvisioningMiddleware>();             // Phase 4
// app.UseAuthorization();                                     // Phase 5

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
```

## Step 1.9 — Set Up `.editorconfig` & Git

```bash
# .editorconfig for consistent code style
dotnet new editorconfig

# Initialize git
git init
echo "bin/\nobj/\n.vs/\n*.user\nappsettings.Development.json" > .gitignore
git add .
git commit -m "chore: initial project setup with Clean Architecture"
```

---

## Verification Checklist

- [ ] Solution builds without errors: `dotnet build IMS.sln`
- [ ] All project references are correct (Domain has no refs, API transitively has all)
- [ ] Folder structure matches the plan
- [ ] `BaseEntity` and `TenantEntity` compile
- [ ] `ApiResponse<T>` and `PagedResponse<T>` compile
- [ ] `dotnet run --project src/IMS.API` starts and shows Swagger at `/swagger`
- [ ] Health check responds at `/health`

---

## Next Phase

→ [Phase 2 — Domain Layer: Entities & Enums](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/02-Domain-Layer-Entities.md)
