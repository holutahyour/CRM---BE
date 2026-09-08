using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationsMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"),
                column: "Position",
                value: 10);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1"),
                column: "Position",
                value: 11);

            migrationBuilder.InsertData(
                table: "accl_menus",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Icon", "IsActive", "IsDeleted", "Label", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name", "ParentId", "Position", "Route" },
                values: new object[] { new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "home", true, false, "Operations", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "OPERATIONS", null, 9, "/operations" });

            migrationBuilder.InsertData(
                table: "accl_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Description", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name" },
                values: new object[,]
                {
                    { new Guid("3490ab1e-2a44-b5e3-c977-e144cd4d8a37"), "operations.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "View Operations" },
                    { new Guid("4c6ac1cc-d1a0-f471-9f5b-56e3fc74fc0b"), "operations.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "Manage Operations" }
                });

            migrationBuilder.InsertData(
                table: "accl_menu_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "MenuId", "PermissionId" },
                values: new object[,]
                {
                    { new Guid("1a869cc7-9fbf-e703-e8f7-3fc7cda75c2b"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"), new Guid("3490ab1e-2a44-b5e3-c977-e144cd4d8a37") },
                    { new Guid("bbe312e4-b5b1-e190-b53f-edcf062c21a5"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"), new Guid("4c6ac1cc-d1a0-f471-9f5b-56e3fc74fc0b") }
                });

            migrationBuilder.InsertData(
                table: "accl_role_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "PermissionId", "RoleId", "TenantId" },
                values: new object[,]
                {
                    { new Guid("0a5e9af5-a98f-c20e-1cd5-185c175fa639"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("3490ab1e-2a44-b5e3-c977-e144cd4d8a37"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("90fb980f-e1b0-9167-fa84-e8f155c02e30"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("4c6ac1cc-d1a0-f471-9f5b-56e3fc74fc0b"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("1a869cc7-9fbf-e703-e8f7-3fc7cda75c2b"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("bbe312e4-b5b1-e190-b53f-edcf062c21a5"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("0a5e9af5-a98f-c20e-1cd5-185c175fa639"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("90fb980f-e1b0-9167-fa84-e8f155c02e30"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("3490ab1e-2a44-b5e3-c977-e144cd4d8a37"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("4c6ac1cc-d1a0-f471-9f5b-56e3fc74fc0b"));

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"),
                column: "Position",
                value: 9);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1"),
                column: "Position",
                value: 10);
        }
    }
}
