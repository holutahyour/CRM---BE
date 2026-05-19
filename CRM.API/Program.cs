using CRM.API.Infrastructure;
using CRM.API.Middleware;
using CRM.Base.Domain.Common;
using CRM.Data;
using CRM.Data.Authorization;
using CRM.Data.Helpers;
using CRM.Domain.Constants;
using CRM.Service;
using CRM.Services.Services.Interfaces.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
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
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CRM", Version = "v1" });
    c.AddSecurityDefinition("AzureAD", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Configure Swagger to use the Azure AD Bearer token security globally
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "AzureAD"
                }
            },
            Array.Empty<string>()
        }
    });
}); builder.Services.AddHealthChecks();

builder.Services
    .AddDataDependencies(builder.Configuration)
    .AddServiceDependencies(builder.Configuration);

builder.Services.AddScoped<ITenantProvider, TenantProvider>();

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

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", p => p.AddRequirements(new PermissionRequirement(Permissions.UsersManage)))
    .AddPolicy("ManagerOrAbove", p => p.AddRequirements(new PermissionRequirement(Permissions.ItemsEdit)))
    .AddPolicy("SupervisorOrAbove", p => p.AddRequirements(new PermissionRequirement(Permissions.ItemsView)))
    .AddPolicy("InventoryRead", p => p.AddRequirements(new PermissionRequirement(Permissions.ItemsView)))
    .AddPolicy("InventoryWrite", p => p.AddRequirements(new PermissionRequirement(Permissions.ItemsCreate)))
    .AddPolicy("PurchaseOrderApprove", p => p.AddRequirements(new PermissionRequirement(Permissions.PurchaseOrdersApprove)))
    .AddPolicy("ReportsView", p => p.AddRequirements(new PermissionRequirement(Permissions.ReportsView)))
    .AddPolicy("WorkflowTemplatesView", p => p.AddRequirements(new PermissionRequirement(Permissions.WorkflowTemplatesView)))
    .AddPolicy("WorkflowTemplatesManage", p => p.AddRequirements(new PermissionRequirement(Permissions.WorkflowTemplatesManage)));

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder
    //.WithOrigins("http://localhost:3000")
    .WithOrigins("*")
    .AllowAnyMethod().AllowAnyHeader();
}));

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
    app.UseSwaggerUI((c =>
    {
        // Enable the "Authorize" button in the Swagger UI
        c.OAuthClientId("swagger");
        c.OAuthClientSecret("secret");
        c.OAuthAppName("Swagger UI");
        c.OAuthUsePkce();
    }));
}

app.UseCors("corsapp");

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

// === Middleware registered in Phases 4-6 ===
app.UseAuthentication();                                    // Phase 4
app.UseMiddleware<TenantMiddleware>();                      // Phase 6
app.UseMiddleware<UserProvisioningMiddleware>();             // Phase 4
app.UseAuthorization();                                     // Phase 5

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();