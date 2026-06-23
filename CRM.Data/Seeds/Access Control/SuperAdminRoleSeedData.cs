using CRM.Domain.Constants;
using System.Security.Cryptography;
using System.Text;

namespace CRM.Data.Seeds;

/// <summary>
/// Seeds the global SUPER_ADMIN role and grants it the <see cref="Permissions.SystemManage"/>
/// permission, which gates super-admin-only catalog controls (module catalog, menu tree,
/// cross-tenant audit view).
/// </summary>
public static class SuperAdminRoleSeedData
{
    public static readonly Guid SuperAdminRoleId = new Guid("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1");

    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = SuperAdminRoleId,
                Name = "Super Administrator",
                Code = "SUPER_ADMIN",
                Description = "Platform super-administrator: manages global catalogs and cross-tenant views.",
                IsSystem = true,
                IsActive = true,
                TenantId = Guid.Empty // Global / system-level role
            }
        );

        var permissionId = CreateDeterministicGuid(Permissions.SystemManage);
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission
            {
                Id = CreateDeterministicGuid($"{SuperAdminRoleId}_{permissionId}"),
                RoleId = SuperAdminRoleId,
                PermissionId = permissionId
            }
        );
    }

    private static Guid CreateDeterministicGuid(string input)
    {
        using MD5 md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return new Guid(hash);
    }
}
