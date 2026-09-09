using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "accl_menus",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Icon", "IsActive", "IsDeleted", "Label", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name", "ParentId", "Position", "Route" },
                values: new object[] { new Guid("c3c3c3c3-3c3c-3c3c-3c3c-c3c3c3c3c3c3"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "trending-up", true, false, "Sales", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "SALES", null, 12, "/sales" });

            migrationBuilder.InsertData(
                table: "accl_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Description", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name" },
                values: new object[,]
                {
                    { new Guid("4c4ed45b-1c5a-7679-2736-3f3b9c7b57a8"), "sales.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "Manage Sales" },
                    { new Guid("e98d58c8-3fe0-4918-79d4-e5d308e6b57d"), "sales.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "View Sales" }
                });

            migrationBuilder.InsertData(
                table: "accl_menu_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "MenuId", "PermissionId" },
                values: new object[,]
                {
                    { new Guid("10906470-4ad1-fc7c-0330-5773ce76bde5"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c3c3c3c3-3c3c-3c3c-3c3c-c3c3c3c3c3c3"), new Guid("4c4ed45b-1c5a-7679-2736-3f3b9c7b57a8") },
                    { new Guid("b68d2af8-d925-9f33-e4dd-e110d7edacee"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c3c3c3c3-3c3c-3c3c-3c3c-c3c3c3c3c3c3"), new Guid("e98d58c8-3fe0-4918-79d4-e5d308e6b57d") }
                });

            migrationBuilder.InsertData(
                table: "accl_role_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "PermissionId", "RoleId", "TenantId" },
                values: new object[,]
                {
                    { new Guid("6edbd770-ecbf-dc1f-e4a2-4064102c5166"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("4c4ed45b-1c5a-7679-2736-3f3b9c7b57a8"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("def79e84-8208-52d4-eba9-41be07cd30c8"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("e98d58c8-3fe0-4918-79d4-e5d308e6b57d"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("10906470-4ad1-fc7c-0330-5773ce76bde5"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("b68d2af8-d925-9f33-e4dd-e110d7edacee"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("6edbd770-ecbf-dc1f-e4a2-4064102c5166"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("def79e84-8208-52d4-eba9-41be07cd30c8"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("c3c3c3c3-3c3c-3c3c-3c3c-c3c3c3c3c3c3"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("4c4ed45b-1c5a-7679-2736-3f3b9c7b57a8"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("e98d58c8-3fe0-4918-79d4-e5d308e6b57d"));
        }
    }
}
