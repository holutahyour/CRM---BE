using CRM.Services.Services.Interfaces.Common;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CRM.Tests.Helpers;

public static class TestDbContext
{
    /// <summary>
    /// Creates an in-memory <see cref="CRM.Data.ApplicationDbContext"/> with a mocked
    /// <see cref="ITenantProvider"/> seeded with the supplied tenant and user identifiers.
    /// Each call uses a unique database name so tests are fully isolated from one another.
    /// </summary>
    public static CRM.Data.ApplicationDbContext Create(
        Guid? tenantId = null,
        string? userId = null)
    {
        var resolvedTenantId = tenantId ?? Guid.NewGuid();
        var resolvedUserId   = userId   ?? "test-user";

        var mockTenantProvider = new Mock<ITenantProvider>();
        mockTenantProvider.Setup(p => p.TenantId).Returns(resolvedTenantId);
        mockTenantProvider.Setup(p => p.UserId).Returns(resolvedUserId);

        var options = new DbContextOptionsBuilder<CRM.Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"CrmTestDb_{Guid.NewGuid()}")
            .Options;

        return new CRM.Data.ApplicationDbContext(options, mockTenantProvider.Object);
    }
}
