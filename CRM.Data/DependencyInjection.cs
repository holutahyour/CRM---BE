namespace CRM.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<OperationalDataSeeder>();
            services.AddScoped<InventoryDataSeeder>();
            services.AddScoped<ProcessingDataSeeder>();
            services.AddScoped<WorkflowDataSeeder>();

            // Provider is selectable per-environment via the "DatabaseProvider" config key.
            // Defaults to SqlServer so existing (production) configuration is unaffected;
            // appsettings.Development.json sets it to "Sqlite" for local development.
            var provider = configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";

            GuardAgainstProviderMismatch(provider, connectionString);

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

        /// <summary>
        /// "DatabaseProvider" and the connection string are resolved from independent config
        /// sources, so they can silently disagree - commenting the SQLite connection string out
        /// of appsettings.Development.json, for example, leaves provider "Sqlite" paired with the
        /// SQL Server connection string inherited from appsettings.json. The provider then fails
        /// deep inside the first database call with "Connection string keyword 'server' is not
        /// supported", which names neither of the two settings that actually have to change.
        /// This turns that into one startup error that does.
        /// </summary>
        private static void GuardAgainstProviderMismatch(string provider, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) return;

            var isSqlite = provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase);

            // Keywords no SQLite connection string ever carries. "Data Source=" is deliberately
            // not one of them - both providers use it.
            var looksLikeSqlServer =
                connectionString.Contains("Server=", StringComparison.OrdinalIgnoreCase)
                || connectionString.Contains("Initial Catalog=", StringComparison.OrdinalIgnoreCase);

            var looksLikeSqlite =
                !looksLikeSqlServer
                && connectionString.Contains("Data Source=", StringComparison.OrdinalIgnoreCase)
                && (connectionString.Contains(".db", StringComparison.OrdinalIgnoreCase)
                    || connectionString.Contains(".sqlite", StringComparison.OrdinalIgnoreCase));

            if (isSqlite && looksLikeSqlServer)
            {
                throw new InvalidOperationException(
                    "DatabaseProvider is \"Sqlite\" but ConnectionStrings:DefaultConnection is a SQL Server " +
                    "connection string. Set DatabaseProvider to \"SqlServer\" to use that database, or restore " +
                    "a SQLite connection string (\"Data Source=crm_dev.db;Foreign Keys=False\") to stay on SQLite. " +
                    "Note that appsettings.Development.json overrides appsettings.json, and environment variables " +
                    "override both.");
            }

            if (!isSqlite && looksLikeSqlite)
            {
                throw new InvalidOperationException(
                    $"DatabaseProvider is \"{provider}\" but ConnectionStrings:DefaultConnection is a SQLite " +
                    "connection string. Set DatabaseProvider to \"Sqlite\", or supply a SQL Server connection string. " +
                    "Note that appsettings.Development.json overrides appsettings.json, and environment variables " +
                    "override both.");
            }
        }
    }
}
