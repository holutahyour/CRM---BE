using Asp.Versioning;
using CRM.Base.Domain.Common;
using CRM.Data;
using CRM.Data.Authorization;
using CRM.Data.Helpers;
using CRM.Domain.Constants;
using CRM.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Serilog;
using System.Text.Json.Serialization;
using static CRM.Data.Helpers.ServiceProviderExtensions;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// === Services registered in Phases 3-6 ===
// builder.Services.AddInfrastructure(builder.Configuration);  // Phase 3

// Add services to the container.
builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services
    .AddDataDependencies(builder.Configuration)
    .AddServiceDependencies(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", p => p.RequireRole("Admin"))
    .AddPolicy("ManagerOrAbove", p => p.RequireRole("Admin", "Manager"))
    .AddPolicy("SupervisorOrAbove", p => p.RequireRole("Admin", "Manager", "Supervisor"))
    .AddPolicy("InventoryRead", p => p.AddRequirements(new PermissionRequirement(Permissions.ItemsView)))
    .AddPolicy("InventoryWrite", p => p.AddRequirements(new PermissionRequirement(Permissions.ItemsCreate)))
    .AddPolicy("PurchaseOrderApprove", p => p.AddRequirements(new PermissionRequirement(Permissions.PurchaseOrdersApprove)))
    .AddPolicy("ReportsView", p => p.AddRequirements(new PermissionRequirement(Permissions.ReportsView)));

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.Services.ApplyMigrationsAndSeed();
}

//for CI/CD production seeding
if (args.Contains("--seed"))
{
    SeedRunner.Run(app.Services);
    return;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

// === Middleware registered in Phases 4-6 ===
// app.UseAuthentication();                                    // Phase 4
// app.UseMiddleware<TenantMiddleware>();                      // Phase 6
// app.UseMiddleware<UserProvisioningMiddleware>();             // Phase 4
// app.UseAuthorization();                                     // Phase 5

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();