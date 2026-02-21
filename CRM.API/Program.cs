using Asp.Versioning;
using Azure.Identity;
using CRM.Data;
using CRM.Data.Helpers;
using CRM.Service;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;
using static CRM.Data.Helpers.ServiceProviderExtensions;

var builder = global::Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
.AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
 });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddScoped<IGetOrganizationConfiguration, GetOrganizationConfiguration>();
//var organizationConfigurationService = builder.Services.BuildServiceProvider().GetService<IGetOrganizationConfiguration>();
//var azureAdConfiguration = organizationConfigurationService.GetAzureAdConfigurationAsync("lagetronix").GetAwaiter().GetResult();

builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration)
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches()
    .AddMicrosoftGraph(x =>
    {
        var clientId = builder.Configuration.GetValue<string>("AzureAd:ClientId");
        var tenantId = builder.Configuration.GetValue<string>("AzureAd:TenantId");
        var clientSecret = builder.Configuration.GetValue<string>("AzureAd:ClientSecret");
        var authorization = builder.Configuration.GetValue<string>("AzureAd:Authority");
        var scope = builder.Configuration.GetValue<string>("AzureAd:Scope");
        var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

        return new GraphServiceClient(clientSecretCredential);
    }, new string[] { ".default" });

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services
    .AddDataDependencies(builder.Configuration)
    .AddServiceDependencies(builder.Configuration);

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

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("corsapp");

app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Tabernacle")
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
        .WithOpenApiRoutePattern("/swagger/v1/swagger.json"); // Add Swagger JSON specification
});

app.UseCors("corsapp");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
