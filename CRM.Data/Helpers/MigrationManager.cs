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
                        WarnAboutMissingSqliteTables(dbContext);
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

        /// <summary>
        /// <c>EnsureCreated</c> builds the schema only when the database file does not exist yet, so a
        /// dev SQLite file created before a new entity was added keeps its old schema forever and the
        /// first write to the new table fails deep inside a request with "no such table".
        /// This turns that into one obvious line at startup naming the tables and the fix.
        /// </summary>
        private static void WarnAboutMissingSqliteTables(DbContext dbContext)
        {
            try
            {
                var expected = dbContext.Model.GetEntityTypes()
                    .Select(e => e.GetTableName())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var command = dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table'";
                    dbContext.Database.OpenConnection();
                    using var reader = command.ExecuteReader();
                    while (reader.Read()) existing.Add(reader.GetString(0));
                }

                var missing = expected.Where(t => !existing.Contains(t!)).ToList();
                if (missing.Count == 0) return;

                Console.WriteLine(
                    $"WARNING: the SQLite database is missing {missing.Count} table(s) from the model: " +
                    $"{string.Join(", ", missing)}. EnsureCreated does not alter an existing file, so writes " +
                    "to these will fail. Delete the SQLite database file and restart to rebuild it " +
                    "(this discards local development data).");
            }
            catch (Exception ex)
            {
                // A diagnostic must never be the reason startup fails.
                Console.WriteLine($"Could not check the SQLite schema for missing tables: {ex.Message}");
            }
        }
    }


}
