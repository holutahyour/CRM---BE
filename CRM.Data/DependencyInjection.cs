namespace CRM.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<OperationalDataSeeder>();
            services.AddScoped<InventoryDataSeeder>();
            services.AddScoped<WorkflowDataSeeder>();

            // Provider is selectable per-environment via the "DatabaseProvider" config key.
            // Defaults to SqlServer so existing (production) configuration is unaffected;
            // appsettings.Development.json sets it to "Sqlite" for local development.
            var provider = configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
                {
                    options.UseSqlite(string.IsNullOrWhiteSpace(connectionString)
                        ? "Data Source=crm_dev.db;Foreign Keys=False"
                        : connectionString);
                }
                else
                {
                    options.UseSqlServer(connectionString,
                        sql => sql.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null));
                }
            });
            //var organizationConfigurationService = services.BuildServiceProvider().GetService<IGetOrganizationConfiguration>();
            //var connectionString = organizationConfigurationService.GetConnectionStringAsync("lagetronix").GetAwaiter().GetResult();

            services.AddScoped(typeof(IMSSQLRepository<,>), typeof(MSSQLRepository<,>));
            services.AddHttpContextAccessor();
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();


            return services;
        }
    }
}
