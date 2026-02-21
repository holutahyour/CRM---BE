using CRM.Services.Services.Interfaces.Common;
using Microsoft.EntityFrameworkCore.Design;

namespace CRM.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private readonly ITenantProvider tenantProvider;

        public ApplicationDbContextFactory(ITenantProvider tenantProvider)
        {
            this.tenantProvider = tenantProvider;
        }

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Build configuration to read from appsettings.json in the API project
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CRM.API"))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // Create DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options, tenantProvider);
        }
    }
}
