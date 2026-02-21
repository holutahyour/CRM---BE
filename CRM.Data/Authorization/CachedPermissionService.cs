using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CRM.Data.Authorization;

public class CachedPermissionService(ApplicationDbContext db, IDistributedCache cache) : IPermissionService
{
    private const int CacheMinutes = 15;

    public async Task<IReadOnlySet<string>> GetUserPermissionsAsync(string entraOid)
    {
        var cacheKey = $"permissions:{entraOid}";
        var cached = await cache.GetStringAsync(cacheKey);

        if (cached != null)
            return JsonSerializer.Deserialize<HashSet<string>>(cached)!;

        var permissions = await db.Users
            .IgnoreQueryFilters()
            .Where(u => u.EntraObjectId == entraOid && !u.IsDeleted)
            .SelectMany(u => u.UserRoles.Where(ur => !ur.IsDeleted))
            .SelectMany(ur => ur.Role.RolePermissions.Where(rp => !rp.IsDeleted))
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync();

        var set = permissions.ToHashSet();
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(set),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheMinutes) });

        return set;
    }

    public async Task InvalidateUserPermissionsAsync(string entraOid)
    {
        await cache.RemoveAsync($"permissions:{entraOid}");
    }
}
