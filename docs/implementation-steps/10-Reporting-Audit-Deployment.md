# Phase 10 — Reporting, Audit & Deployment

> **Goal**: Implement reporting endpoints, configure logging/monitoring, set up CI/CD, and deploy to Azure.

---

## Step 10.1 — Reporting Endpoints

### Controller: `ReportsController`

| Method | Endpoint                                        | Policy          | Description                       |
| ------ | ----------------------------------------------- | --------------- | --------------------------------- |
| `GET`  | `/api/reports/stock-summary`                    | `ReportsView`   | Current stock by item/location    |
| `GET`  | `/api/reports/stock-valuation`                  | `ReportsView`   | Total inventory value             |
| `GET`  | `/api/reports/expiration-alerts`                | `ReportsView`   | Items expiring in N days          |
| `GET`  | `/api/reports/movement-history`                 | `ReportsView`   | Transaction log with filters      |
| `GET`  | `/api/reports/sales-trends`                     | `ReportsView`   | Sales grouped by period           |
| `GET`  | `/api/reports/supplier-performance`             | `ReportsView`   | On-time delivery, order fill rate |
| `GET`  | `/api/reports/batch-traceability/{batchNumber}` | `ReportsView`   | Full batch lifecycle              |
| `POST` | `/api/reports/export`                           | `ReportsExport` | Export to CSV/Excel               |

### Stock Summary Query

```csharp
[HttpGet("stock-summary")]
public async Task<IActionResult> StockSummary([FromQuery] int? categoryId, [FromQuery] int? locationId)
{
    var query = _db.ItemLocations
        .Include(il => il.Item).ThenInclude(i => i.Category)
        .Include(il => il.Location)
        .AsQueryable();

    if (categoryId.HasValue) query = query.Where(il => il.Item.CategoryId == categoryId);
    if (locationId.HasValue) query = query.Where(il => il.LocationId == locationId);

    var result = await query.Select(il => new StockSummaryDto(
        il.Item.Sku, il.Item.Name, il.Item.Category!.Name,
        il.Location.Name, il.Quantity, il.Reserved, il.Damaged,
        il.Quantity - il.Reserved - il.Damaged,
        il.Item.MinStockLevel,
        (il.Quantity - il.Reserved - il.Damaged) <= (il.Item.MinStockLevel ?? 0)
    )).ToListAsync();

    return Ok(new ApiResponse<List<StockSummaryDto>>(true, result));
}
```

### Batch Traceability Query

```csharp
[HttpGet("batch-traceability/{batchNumber}")]
public async Task<IActionResult> BatchTraceability(string batchNumber)
{
    var batch = await _db.Batches.Include(b => b.Item)
        .FirstOrDefaultAsync(b => b.BatchNumber == batchNumber);
    if (batch == null) return NotFound();

    var transactions = await _db.InventoryTransactions
        .Where(t => t.BatchId == batch.Id)
        .OrderBy(t => t.TransactionDate)
        .ToListAsync();

    return Ok(new ApiResponse<BatchTraceabilityDto>(true, new(batch, transactions)));
}
```

### CSV/Excel Export

```csharp
[HttpPost("export")]
public async Task<IFileResult> Export([FromBody] ExportRequest request)
{
    // Use a library like ClosedXML or CsvHelper
    // Stream the file to avoid large memory allocations
    var stream = request.Format switch
    {
        "csv" => await GenerateCsv(request),
        "xlsx" => await GenerateExcel(request),
        _ => throw new ArgumentException("Unsupported format")
    };

    return File(stream, "application/octet-stream", $"{request.ReportType}_{DateTime.UtcNow:yyyyMMdd}.{request.Format}");
}
```

NuGet packages: `CsvHelper` for CSV, `ClosedXML` for Excel.

---

## Step 10.2 — Serilog Configuration

**File**: `src/IMS.API/appsettings.json` — add Serilog section:

```jsonc
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.ApplicationInsights"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning",
      },
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "ApplicationInsights",
        "Args": {
          "connectionString": "<from-config>",
          "telemetryConverter": "Serilog.Sinks.ApplicationInsights.TelemetryConverters.TraceTelemetryConverter, Serilog.Sinks.ApplicationInsights",
        },
      },
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"],
    "Properties": { "Application": "IMS-API" },
  },
}
```

### Request Logging Enhancement

```csharp
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("TenantId", httpContext.Items["TenantId"]?.ToString() ?? "unknown");
        diagnosticContext.Set("UserId", httpContext.User.FindFirst("oid")?.Value ?? "anonymous");
    };
});
```

---

## Step 10.3 — Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "database")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!, name: "redis")
    .AddAzureBlobStorage(builder.Configuration["BlobStorage:ConnectionString"]!, name: "blob-storage");

// Install packages:
// Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
// AspNetCore.HealthChecks.SqlServer
// AspNetCore.HealthChecks.Redis
// AspNetCore.HealthChecks.AzureStorage
```

Map with UI:

```csharp
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

---

## Step 10.4 — Application Insights

```csharp
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);
```

Custom telemetry for key operations:

```csharp
public class StockOperationTelemetryFilter : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        if (telemetry is RequestTelemetry request)
        {
            // Add tenant context to all telemetry
            if (request.Context.GlobalProperties.ContainsKey("TenantId"))
                return;
            request.Context.GlobalProperties["TenantId"] = "unknown";
        }
    }
}
```

---

## Step 10.5 — CI/CD Pipeline (GitHub Actions)

**File**: `.github/workflows/ci-cd.yml`

```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

env:
  DOTNET_VERSION: "8.x"
  AZURE_WEBAPP_NAME: "ims-api"

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: "${{ env.DOTNET_VERSION }}" }
      - run: dotnet restore IMS.sln
      - run: dotnet build IMS.sln --no-restore -c Release
      - run: dotnet test tests/IMS.UnitTests --no-build -c Release --logger trx
      - run: dotnet test tests/IMS.IntegrationTests --no-build -c Release --logger trx

  deploy-staging:
    needs: build-and-test
    if: github.ref == 'refs/heads/develop'
    runs-on: ubuntu-latest
    environment: staging
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: "${{ env.DOTNET_VERSION }}" }
      - run: dotnet publish src/IMS.API -c Release -o ./publish
      - uses: azure/webapps-deploy@v3
        with:
          app-name: "${{ env.AZURE_WEBAPP_NAME }}-staging"
          package: ./publish

  deploy-production:
    needs: build-and-test
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    environment: production
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: "${{ env.DOTNET_VERSION }}" }
      - run: dotnet publish src/IMS.API -c Release -o ./publish
      - uses: azure/webapps-deploy@v3
        with:
          app-name: "${{ env.AZURE_WEBAPP_NAME }}"
          package: ./publish
```

---

## Step 10.6 — Azure Infrastructure

### Required Resources

| Service                    | SKU               | Purpose                               |
| -------------------------- | ----------------- | ------------------------------------- |
| Azure App Service          | B1+ (scale to S1) | Host API                              |
| Azure SQL Database         | S0/S1             | Primary database                      |
| Azure Redis Cache          | C0 (Basic)        | Permission caching, sessions          |
| Azure Blob Storage         | Standard          | File uploads (images)                 |
| Azure Key Vault            | Standard          | Secrets management                    |
| Azure Application Insights | Pay-as-you-go     | Monitoring & logging                  |
| Azure API Management       | Consumption       | Rate limiting, API gateway (optional) |

### Key Vault Integration

```csharp
// In Program.cs
if (!builder.Environment.IsDevelopment())
{
    var keyVaultUri = new Uri(builder.Configuration["KeyVault:Url"]!);
    builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
}
```

### CORS Configuration

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

app.UseCors("Frontend");
```

---

## Step 10.7 — Testing Strategy

### Unit Tests (IMS.UnitTests)

- Test MediatR handlers with mocked repositories
- Test FluentValidation validators
- Test domain logic (stock calculations, order status transitions)
- Test permission service logic

### Integration Tests (IMS.IntegrationTests)

- Use `WebApplicationFactory` + Testcontainers for SQL Server
- Test full API endpoint flows (create → read → update → delete)
- Test tenant isolation (ensure tenant A cannot see tenant B's data)
- Test auth scenarios (authenticated vs unauthenticated, authorized vs unauthorized)

```csharp
public class ItemsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetItems_Authenticated_ReturnsOk()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetTestToken());
        var response = await client.GetAsync("/api/items");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

---

## Verification Checklist

- [ ] Stock summary report returns correct aggregated data
- [ ] Expiration alert shows items within configurable threshold
- [ ] Batch traceability returns full lifecycle (receive → transfer → sale)
- [ ] CSV and Excel exports generate valid files
- [ ] Serilog logs include TenantId and UserId in all entries
- [ ] Health check endpoint reports status of DB, Redis, Blob
- [ ] CI/CD pipeline builds, tests, and deploys successfully
- [ ] Application Insights captures requests, exceptions, and custom metrics
- [ ] CORS allows frontend origin only
- [ ] Key Vault integration works in production environment

---

## 🎉 Implementation Complete

You have completed all 10 phases of the IMS implementation. Return to the [Master Overview](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/00-Implementation-Overview.md) for the full index.
