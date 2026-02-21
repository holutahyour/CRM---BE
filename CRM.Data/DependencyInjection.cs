namespace CRM.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<OperationalDataSeeder>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") ?? "");
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
