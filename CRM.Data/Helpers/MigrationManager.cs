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
                    // The migrations in this project are generated for SqlServer and cannot be
                    // applied to SQLite. For local SQLite development we build the schema directly
                    // from the model instead (same approach as the SQLite test harness).
                    if (dbContext.Database.IsSqlite())
                    {
                        dbContext.Database.EnsureCreated();
                        result.SetSuccess($"Ensured SQLite schema for {typeof(TContext).Name}.", "SQLite schema created.");
                    }
                    else
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
