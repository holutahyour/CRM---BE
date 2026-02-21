using CRM.Base.Common.Domain.Common;

namespace CRM.Data.Helpers
{
    public static class MigrationManager
    {
        private static readonly object _lock = new();

        public static Result<string> ApplyMigrations<TContext>(IServiceProvider serviceProvider)
            where TContext : DbContext
        {
            var result = new Result<string>(true);

            lock (_lock)
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

                try
                {
                    var pending = dbContext.Database.GetPendingMigrations();
                    if (pending.Any())
                    {
                        dbContext.Database.Migrate();
                        result.SetSuccess($"Applied {pending.Count()} migrations for {typeof(TContext).Name}.", "Migrations applied successfully.");
                    }
                    else
                    {
                        result.SetSuccess($"No pending migrations for {typeof(TContext).Name}.", "No migrations needed.");
                    }
                }
                catch (Exception ex)
                {
                    result.SetError(ex.ToString(), $"Failed to apply migrations for {typeof(TContext).Name}.");
                }
            }

            return result;
        }
    }


}
