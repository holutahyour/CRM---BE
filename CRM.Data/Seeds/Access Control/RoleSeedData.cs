using CRM.Domain.Entities;
using CRM.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CRM.Data.Seeds;

public static class RoleSeedData
{
    public static readonly Guid AdminRoleId = new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0");

    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = AdminRoleId,
                Name = "Administrator",
                Code = "ADMIN",
                Description = "Full system access with all permissions.",
                IsSystem = true,
                IsActive = true,
                TenantId = Guid.Empty // System-level or global template role
            }
        );
    }
}

public static class RolePermissionSeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var rolePermissions = new List<RolePermission>();

        // Link EVERY permission to the Administrator role
        foreach (var code in Permissions.All)
        {
            var permissionId = CreateDeterministicGuid(code);
            rolePermissions.Add(new RolePermission
            {
                Id = CreateDeterministicGuid($"{RoleSeedData.AdminRoleId}_{permissionId}"),
                RoleId = RoleSeedData.AdminRoleId,
                PermissionId = permissionId,
            });
        }

        modelBuilder.Entity<RolePermission>().HasData(rolePermissions.ToArray());
    }

    private static Guid CreateDeterministicGuid(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return new Guid(hash);
        }
    }
}
