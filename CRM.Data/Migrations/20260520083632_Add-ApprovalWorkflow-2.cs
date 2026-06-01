using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalWorkflow2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "accl_menus",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Icon", "IsActive", "IsDeleted", "Label", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name", "ParentId", "Position", "Route" },
                values: new object[] { new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "git-branch", true, false, "Approval Workflows", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "APPROVAL_WORKFLOWS", null, 10, "/approval-workflows" });

            migrationBuilder.InsertData(
                table: "accl_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Description", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name" },
                values: new object[,]
                {
                    { new Guid("2d3a84df-456e-34fc-34cc-4b13733b5d9e"), "workflows.templates.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "Manage Templates" },
                    { new Guid("da68ffab-1319-895c-e664-1a35535fc7fa"), "workflows.templates.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "View Templates" }
                });

            migrationBuilder.InsertData(
                table: "accl_menu_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "MenuId", "PermissionId" },
                values: new object[,]
                {
                    { new Guid("0932e001-cd97-b94d-9517-39ba9f0b2b2e"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1"), new Guid("da68ffab-1319-895c-e664-1a35535fc7fa") },
                    { new Guid("ee56de02-3bd9-5146-b6f2-916b4e855ccf"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1"), new Guid("2d3a84df-456e-34fc-34cc-4b13733b5d9e") }
                });

            migrationBuilder.InsertData(
                table: "accl_role_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "PermissionId", "RoleId", "TenantId" },
                values: new object[,]
                {
                    { new Guid("36961c40-cf98-1ad0-5162-7d6bfb90b339"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("da68ffab-1319-895c-e664-1a35535fc7fa"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("44cffe22-28f3-7136-658c-000d34cd75e0"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("2d3a84df-456e-34fc-34cc-4b13733b5d9e"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("0932e001-cd97-b94d-9517-39ba9f0b2b2e"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("ee56de02-3bd9-5146-b6f2-916b4e855ccf"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("36961c40-cf98-1ad0-5162-7d6bfb90b339"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("44cffe22-28f3-7136-658c-000d34cd75e0"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("2d3a84df-456e-34fc-34cc-4b13733b5d9e"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("da68ffab-1319-895c-e664-1a35535fc7fa"));
        }
    }
}
