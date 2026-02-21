using CRM.Base.Common.Domain.Common;

namespace CRM.Data.Helpers
{
    public static class ServiceProviderExtensions
    {
        public static void ApplyMigrationsAndSeed(this IServiceProvider serviceProvider)
        {
            var result = new Result<string>(true);
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var migrationResult = MigrationManager.ApplyMigrations<ApplicationDbContext>(serviceProvider);

            if (migrationResult.HasError)
            {
                Console.WriteLine($"Migration error: {migrationResult.ErrorMessage}");
                throw new Exception(migrationResult.ErrorMessage);
            }
            else
            {
                Console.WriteLine(migrationResult.Message);
            }

            new Seeder(context).Intialize();
        }

        public static class SeedRunner
        {
            public static void Run(IServiceProvider services)
            {
                using var scope = services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                new Seeder(context).Intialize();
            }
        }

    }

}
