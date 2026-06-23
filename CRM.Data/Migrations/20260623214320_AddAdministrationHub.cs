using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdministrationHub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "cor_audit_logs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "accl_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Description", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name" },
                values: new object[] { new Guid("c878a896-bbd3-fd79-31e3-2b43c6fa1f58"), "admin.system.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "Manage System" });

            migrationBuilder.InsertData(
                table: "accl_roles",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Description", "IsActive", "IsDeleted", "IsSystem", "LastModifiedBy", "LastModifiedOn", "Name", "TenantId" },
                values: new object[] { new Guid("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1"), "SUPER_ADMIN", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Platform super-administrator: manages global catalogs and cross-tenant views.", true, false, true, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Super Administrator", new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "accl_role_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "PermissionId", "RoleId", "TenantId" },
                values: new object[,]
                {
                    { new Guid("1be3c3f5-4d45-1e62-39d0-9a891ecea5c5"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c878a896-bbd3-fd79-31e3-2b43c6fa1f58"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("843552b7-3281-5bff-caf1-1fde4ed6202d"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c878a896-bbd3-fd79-31e3-2b43c6fa1f58"), new Guid("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("1be3c3f5-4d45-1e62-39d0-9a891ecea5c5"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("843552b7-3281-5bff-caf1-1fde4ed6202d"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("c878a896-bbd3-fd79-31e3-2b43c6fa1f58"));

            migrationBuilder.DeleteData(
                table: "accl_roles",
                keyColumn: "Id",
                keyValue: new Guid("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1"));

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "cor_audit_logs");
        }
    }
}
