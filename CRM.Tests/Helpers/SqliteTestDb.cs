using CRM.Services.Services.Interfaces.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CRM.Tests.Helpers;

/// <summary>
/// A disposable, isolated <see cref="CRM.Data.ApplicationDbContext"/> backed by an
/// <b>in-memory SQLite</b> database. Unlike the EF InMemory provider, SQLite is a real relational
/// engine, so it exercises actual SQL translation, foreign keys, unique indexes, transactions and
/// column types — catching issues the InMemory provider silently ignores.
///
/// Usage:
/// <code>
/// using var sqlite = SqliteTestDb.Create(tenantId);
/// var db = sqlite.Context;
/// // ... arrange / act / assert ...
/// </code>
///
/// The SQLite connection is kept open for the lifetime of this object — an in-memory database is
/// dropped the moment its last connection closes — and is disposed together with the context.
/// </summary>
public sealed class SqliteTestDb : IDisposable
{
    private readonly SqliteConnection _connection;

    public CRM.Data.ApplicationDbContext Context { get; }

    private SqliteTestDb(SqliteConnection connection, CRM.Data.ApplicationDbContext context)
    {
        _connection = connection;
        Context = context;
    }

    /// <summary>
    /// Creates a fresh in-memory SQLite database (schema built from the model via
    /// <c>EnsureCreated()</c>, including the static <c>HasData</c> reference seed) with a mocked
    /// <see cref="ITenantProvider"/> for the supplied tenant/user.
    /// </summary>
    public static SqliteTestDb Create(Guid? tenantId = null, string? userId = null)
    {
        var resolvedTenantId = tenantId ?? Guid.NewGuid();
        var resolvedUserId = userId ?? "test-user";

        var mockTenantProvider = new Mock<ITenantProvider>();
        mockTenantProvider.Setup(p => p.TenantId).Returns(resolvedTenantId);
        mockTenantProvider.Setup(p => p.UserId).Returns(resolvedUserId);

        // Foreign Keys=False: the context's static HasData seed (Modules/Menus) contains
        // self-referencing/hierarchical rows that SQLite cannot insert under strict FK ordering,
        // so FK enforcement is disabled to let EnsureCreated build the schema + reference seed.
        // SQL translation, column types and UNIQUE indexes are still fully enforced.
        var connection = new SqliteConnection("DataSource=:memory:;Foreign Keys=False");
        connection.Open();

        var options = new DbContextOptionsBuilder<CRM.Data.ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new CRM.Data.ApplicationDbContext(options, mockTenantProvider.Object);
        context.Database.EnsureCreated();

        return new SqliteTestDb(connection, context);
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
