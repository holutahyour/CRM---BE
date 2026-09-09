using CRM.Data;
using CRM.Data.Seeds;
using CRM.Domain.Entities;
using CRM.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CRM.Tests.Services;

/// <summary>
/// The model seeds two global roles via <c>HasData</c> — ADMIN (<see cref="RoleSeedData.AdminRoleId"/>)
/// and SUPER_ADMIN (<see cref="SuperAdminRoleSeedData.SuperAdminRoleId"/>) — both pinned to
/// <c>TenantId = Guid.Empty</c>. No tenant with that id is ever created: <see cref="Seeder"/> asks for
/// <c>Id = Guid.Empty</c> but <c>BaseEntity.Id</c> is <c>DatabaseGeneratedOption.Identity</c>, so EF
/// discards the CLR-default value and generates a random one. The global roles are therefore orphaned,
/// and the seeder adds a *second* ADMIN under the real SYSTEM tenant. These tests pin the reconciliation
/// that collapses the duplicates onto the SYSTEM tenant.
/// </summary>
public class SeederGlobalRoleReconcileTests
{
    private static (SqliteTestDb sqlite, ApplicationDbContext db, Guid systemTenantId) Setup()
    {
        var sqlite = SqliteTestDb.Create();
        var db = sqlite.Context;

        // The SYSTEM tenant as the real seeder produces it: a generated, non-empty id.
        var tenant = new Tenant { Name = "System", Code = "SYSTEM" };
        db.Tenants.Add(tenant);
        db.SaveChanges();
        tenant.Id.Should().NotBe(Guid.Empty, "EF generates the key, which is the whole cause of this bug");

        return (sqlite, db, tenant.Id);
    }

    [Fact]
    public void Reconcile_repoints_orphaned_global_roles_onto_the_system_tenant()
    {
        var (sqlite, db, systemTenantId) = Setup();
        using var _ = sqlite;

        new Seeder(db).ReconcileGlobalRoles();
        db.ChangeTracker.Clear();

        var orphans = db.Roles.IgnoreQueryFilters()
            .Where(r => !r.IsDeleted && r.TenantId == Guid.Empty)
            .ToList();
        orphans.Should().BeEmpty("no live role may belong to a tenant that does not exist");

        var admin = db.Roles.IgnoreQueryFilters().Single(r => r.Id == RoleSeedData.AdminRoleId);
        admin.TenantId.Should().Be(systemTenantId);

        var superAdmin = db.Roles.IgnoreQueryFilters().Single(r => r.Id == SuperAdminRoleSeedData.SuperAdminRoleId);
        superAdmin.TenantId.Should().Be(systemTenantId);
    }

    [Fact]
    public void Reconcile_retires_the_orphan_when_the_tenant_already_owns_a_role_with_that_code()
    {
        var (sqlite, db, systemTenantId) = Setup();
        using var _ = sqlite;

        // What an already-seeded database looks like: Seeder added its own ADMIN under the real tenant,
        // leaving the HasData ADMIN stranded at Guid.Empty. Two live ADMIN roles.
        var tenantOwnedAdmin = new Role
        {
            Id = Guid.NewGuid(), TenantId = systemTenantId,
            Name = "Administrator", Code = "ADMIN", IsSystem = true
        };
        db.Roles.Add(tenantOwnedAdmin);
        db.SaveChanges();

        new Seeder(db).ReconcileGlobalRoles();
        db.ChangeTracker.Clear();

        var liveAdmins = db.Roles.IgnoreQueryFilters()
            .Where(r => r.Code == "ADMIN" && !r.IsDeleted)
            .ToList();
        liveAdmins.Should().ContainSingle("the duplicate ADMIN is what makes role lookup non-deterministic");
        liveAdmins[0].Id.Should().Be(tenantOwnedAdmin.Id, "the tenant-owned role survives; the orphan is retired");
    }

    [Fact]
    public void Reconcile_moves_user_assignments_off_a_retired_orphan_role()
    {
        var (sqlite, db, systemTenantId) = Setup();
        using var _ = sqlite;

        var tenantOwnedAdmin = new Role
        {
            Id = Guid.NewGuid(), TenantId = systemTenantId,
            Name = "Administrator", Code = "ADMIN", IsSystem = true
        };
        var user = new User
        {
            Id = Guid.NewGuid(), TenantId = systemTenantId,
            EntraObjectId = "oid-1", Email = "admin@example.com"
        };
        // AssignAdminRoleAsync matched the orphan (TenantId == Guid.Empty) and bound the user to it.
        var assignment = new UserRole
        {
            Id = Guid.NewGuid(), TenantId = systemTenantId,
            UserId = user.Id, RoleId = RoleSeedData.AdminRoleId
        };
        db.Roles.Add(tenantOwnedAdmin);
        db.Users.Add(user);
        db.UserRoles.Add(assignment);
        db.SaveChanges();

        new Seeder(db).ReconcileGlobalRoles();
        db.ChangeTracker.Clear();

        var reassigned = db.UserRoles.IgnoreQueryFilters().Single(ur => ur.Id == assignment.Id);
        reassigned.RoleId.Should().Be(tenantOwnedAdmin.Id,
            "otherwise the user is left pointing at a filtered-out role and silently loses every permission");
    }

    [Fact]
    public void Reconcile_is_idempotent()
    {
        var (sqlite, db, systemTenantId) = Setup();
        using var _ = sqlite;

        var seeder = new Seeder(db);
        seeder.ReconcileGlobalRoles();
        db.ChangeTracker.Clear();
        seeder.ReconcileGlobalRoles();
        db.ChangeTracker.Clear();

        db.Roles.IgnoreQueryFilters()
            .Where(r => r.Code == "ADMIN" && !r.IsDeleted)
            .Should().ContainSingle();
        db.Roles.IgnoreQueryFilters()
            .Where(r => r.Code == "SUPER_ADMIN" && !r.IsDeleted)
            .Should().ContainSingle();
    }
}
