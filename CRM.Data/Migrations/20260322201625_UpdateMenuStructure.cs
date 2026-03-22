using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMenuStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("00344a75-ff2e-2874-569e-baaa7c7109d3"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("0519010f-24a0-d469-9623-9389456dc15e"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("0c8e43cd-a64d-76d1-02a4-88ac8e8c8845"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("1b04d704-9398-dcec-74f4-8ea315a5056c"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("30a7c24f-24b3-229d-f2da-62f3a22e47cf"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("3c517ad2-064b-b36b-c774-7fa2c4212383"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("4fb2a604-9441-d43b-d9f3-ed310c93e290"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("54d48df9-b29b-8b59-35b0-4554ab7db644"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("6564aafb-c32f-9241-db36-2b7248794e6d"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("6e9b3035-ce9e-73f3-734f-597d8f9c477c"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("7416247e-12f3-f95c-959d-5eee316a70bb"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("84926b1e-6f88-eeea-8bf0-73c86e2a3e47"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("8d08d4b4-c35f-7b8d-9afe-b8322f94ba3d"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("9bc03684-568f-ab12-7bea-fac623ab5190"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("a1931099-0a4b-295e-1c54-f9016e9ab5f3"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("ac1d266b-afe5-85a4-f13b-4f9417636965"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("b9ec6448-8bb8-f0aa-2156-1d9d8a8b1ce5"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("c12a8a57-1009-a25f-fb60-dbfb582043cd"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("cfa93872-5de0-c245-94c3-b1b881f80d5f"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("e8365eb4-db49-bf4a-a604-1b4d1afd3830"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("f1d52b69-e129-58ca-3c56-179948a2960e"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("1f1f1f1f-1f1f-1f1f-1f1f-1f1f1f1f1f1f"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("d2d2d2d2-2d2d-2d2d-2d2d-d2d2d2d2d2d2"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("d3d3d3d3-3d3d-3d3d-3d3d-d3d3d3d3d3d3"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("e1e1e1e1-1e1e-1e1e-1e1e-e1e1e1e1e1e1"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("e2e2e2e2-2e2e-2e2e-2e2e-e2e2e2e2e2e2"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("e3e3e3e3-3e3e-3e3e-3e3e-e3e3e3e3e3e3"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("f1f1f1f1-1f1f-1f1f-1f1f-f1f1f1f1f1f1"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("f2f2f2f2-2f2f-2f2f-2f2f-f2f2f2f2f2f2"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("f3f3f3f3-3f3f-3f3f-3f3f-f3f3f3f3f3f3"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("f4f4f4f4-4f4f-4f4f-4f4f-f4f4f4f4f4f4"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("f5f5f5f5-5f5f-5f5f-5f5f-f5f5f5f5f5f5"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.InsertData(
                table: "accl_menu_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "MenuId", "PermissionId" },
                values: new object[] { new Guid("fa374732-77cc-85e2-4588-5401ea35eeb8"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d1d1d1d1-1d1d-1d1d-1d1d-d1d1d1d1d1d1"), new Guid("a6144ccf-d362-4ed1-cb01-8207890bea3a") });

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "Icon", "Position" },
                values: new object[] { "archive", 4 });

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("2f2f2f2f-2f2f-2f2f-2f2f-2f2f2f2f2f2f"),
                column: "Position",
                value: 0);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("3f3f3f3f-3f3f-3f3f-3f3f-3f3f3f3f3f3f"),
                column: "Position",
                value: 1);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("4f4f4f4f-4f4f-4f4f-4f4f-4f4f4f4f4f4f"),
                column: "Position",
                value: 2);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("5f5f5f5f-5f5f-5f5f-5f5f-5f5f5f5f5f5f"),
                column: "Position",
                value: 3);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("6f6f6f6f-6f6f-6f6f-6f6f-6f6f6f6f6f6f"),
                column: "Position",
                value: 4);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"),
                column: "Position",
                value: 8);

            migrationBuilder.InsertData(
                table: "accl_menus",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Icon", "IsActive", "IsDeleted", "Label", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name", "ParentId", "Position", "Route" },
                values: new object[,]
                {
                    { new Guid("a0a0a0a0-0a0a-0a0a-0a0a-a0a0a0a0a0a0"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "file-text", true, false, "Requisitions", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "REQUISITIONS", null, 1, "/requisitions" },
                    { new Guid("b0b0b0b0-0b0b-0b0b-0b0b-b0b0b0b0b0b0"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "package", true, false, "Item Requests", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "ITEM_REQUESTS", null, 2, "/item-requests" },
                    { new Guid("c0c0c0c0-0c0c-0c0c-0c0c-c0c0c0c0c0c0"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bar-chart-2", true, false, "Monthly Reports", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "REPORTS", "MONTHLY_REPORTS", null, 3, "/reports/monthly" },
                    { new Guid("d0d0d0d0-0d0d-0d0d-0d0d-d0d0d0d0d0d0"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "alert-circle", true, false, "Incident Reports", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "INCIDENT_REPORTS", null, 5, "/incidents" },
                    { new Guid("e0e0e0e0-0e0e-0e0e-0e0e-e0e0e0e0e0e0"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "users", true, false, "User Management", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "USER_MANAGEMENT", null, 6, "/admin/users" },
                    { new Guid("f0f0f0f0-0f0f-0f0f-0f0f-f0f0f0f0f0f0"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "building-2", true, false, "Departments", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "DEPARTMENTS", null, 7, "/departments" }
                });

            migrationBuilder.InsertData(
                table: "accl_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Description", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name" },
                values: new object[,]
                {
                    { new Guid("07b69eec-3d2d-cbc5-6fd9-824559c15c21"), "incidents.resolve", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "Resolve Incidents" },
                    { new Guid("145982d7-9af7-2943-e9ab-639b3972f84d"), "itemrequests.create", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "Create Itemrequests" },
                    { new Guid("1dbad435-1f48-7a2e-6353-8c4f135ad3cf"), "departments.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "View Departments" },
                    { new Guid("27f71bec-8fdb-bd8b-7f43-719e72b591bf"), "itemrequests.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "View Itemrequests" },
                    { new Guid("2f103dbd-cf76-5f45-b619-a3bc3656e0f8"), "requisitions.approve", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "Approve Requisitions" },
                    { new Guid("37d70f0e-4b74-7001-95db-139216845aa0"), "requisitions.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "View Requisitions" },
                    { new Guid("515e8274-7978-7bf1-bf92-04e2d60bd874"), "incidents.create", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "Create Incidents" },
                    { new Guid("61fbd42f-6d0e-f433-0c79-1bb9186ae253"), "reports.monthly.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "REPORTS", "View Monthly" },
                    { new Guid("90002ea6-55f1-fdd1-637b-2d315c2e566d"), "departments.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "Manage Departments" },
                    { new Guid("ccdc1260-220f-3e1b-2122-d125ce548976"), "requisitions.create", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "Create Requisitions" },
                    { new Guid("d74f385f-999c-f66f-2034-604ff8d5269c"), "incidents.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "View Incidents" }
                });

            migrationBuilder.InsertData(
                table: "accl_menu_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "MenuId", "PermissionId" },
                values: new object[,]
                {
                    { new Guid("1093e7cd-e261-70ee-e0d5-f0f960f8b22d"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("b0b0b0b0-0b0b-0b0b-0b0b-b0b0b0b0b0b0"), new Guid("27f71bec-8fdb-bd8b-7f43-719e72b591bf") },
                    { new Guid("5a2419bd-b6f6-d4cc-415a-ba0d88a96378"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("f0f0f0f0-0f0f-0f0f-0f0f-f0f0f0f0f0f0"), new Guid("1dbad435-1f48-7a2e-6353-8c4f135ad3cf") },
                    { new Guid("81ca8e94-0732-f2a8-798a-438dd8c65d88"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("e0e0e0e0-0e0e-0e0e-0e0e-e0e0e0e0e0e0"), new Guid("a6144ccf-d362-4ed1-cb01-8207890bea3a") },
                    { new Guid("823d58d4-565d-dca1-8c69-ab881fddfee3"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c0c0c0c0-0c0c-0c0c-0c0c-c0c0c0c0c0c0"), new Guid("61fbd42f-6d0e-f433-0c79-1bb9186ae253") },
                    { new Guid("bfac4efe-b040-f079-ca1a-91f3b664a118"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d0d0d0d0-0d0d-0d0d-0d0d-d0d0d0d0d0d0"), new Guid("d74f385f-999c-f66f-2034-604ff8d5269c") },
                    { new Guid("d3587342-c2df-d73f-6202-ac33f92cd05c"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("a0a0a0a0-0a0a-0a0a-0a0a-a0a0a0a0a0a0"), new Guid("37d70f0e-4b74-7001-95db-139216845aa0") },
                    { new Guid("f3040e78-1fe9-f972-83fb-79b0cc03162b"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("e0e0e0e0-0e0e-0e0e-0e0e-e0e0e0e0e0e0"), new Guid("bbbfe961-127d-5c27-bd5e-babc2e2b0a6c") }
                });

            migrationBuilder.InsertData(
                table: "accl_role_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "PermissionId", "RoleId", "TenantId" },
                values: new object[,]
                {
                    { new Guid("124d9088-8b1d-6c1b-05e8-2bdb8686a478"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("ccdc1260-220f-3e1b-2122-d125ce548976"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("39ec8bba-9341-5dce-fd58-05285ffee050"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("07b69eec-3d2d-cbc5-6fd9-824559c15c21"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("57a3ea05-5817-9ccf-8eea-a714b6568cc5"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("2f103dbd-cf76-5f45-b619-a3bc3656e0f8"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("756d613c-67bc-f6c6-fccc-b0c9ed986259"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("1dbad435-1f48-7a2e-6353-8c4f135ad3cf"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("7e873ed6-56f5-6ecc-c4e2-51db21ab47fb"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("145982d7-9af7-2943-e9ab-639b3972f84d"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("907eb9b6-2c90-ed71-6c0a-dbada9b350f8"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("515e8274-7978-7bf1-bf92-04e2d60bd874"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("b2a0243a-042f-894e-3919-1083b0180841"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("61fbd42f-6d0e-f433-0c79-1bb9186ae253"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("c9ea40b7-f594-86b7-2d20-b1f9748ecc20"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("37d70f0e-4b74-7001-95db-139216845aa0"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("e8120980-02b3-9587-8349-9a4b4dc7ebfa"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d74f385f-999c-f66f-2034-604ff8d5269c"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("eac98a98-b715-6af2-f86a-c7d73629e847"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("27f71bec-8fdb-bd8b-7f43-719e72b591bf"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("fc71dba4-de48-2c0e-1927-96d00fa3a976"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("90002ea6-55f1-fdd1-637b-2d315c2e566d"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("1093e7cd-e261-70ee-e0d5-f0f960f8b22d"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("5a2419bd-b6f6-d4cc-415a-ba0d88a96378"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("81ca8e94-0732-f2a8-798a-438dd8c65d88"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("823d58d4-565d-dca1-8c69-ab881fddfee3"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("bfac4efe-b040-f079-ca1a-91f3b664a118"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("d3587342-c2df-d73f-6202-ac33f92cd05c"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("f3040e78-1fe9-f972-83fb-79b0cc03162b"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("fa374732-77cc-85e2-4588-5401ea35eeb8"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("124d9088-8b1d-6c1b-05e8-2bdb8686a478"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("39ec8bba-9341-5dce-fd58-05285ffee050"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("57a3ea05-5817-9ccf-8eea-a714b6568cc5"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("756d613c-67bc-f6c6-fccc-b0c9ed986259"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("7e873ed6-56f5-6ecc-c4e2-51db21ab47fb"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("907eb9b6-2c90-ed71-6c0a-dbada9b350f8"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("b2a0243a-042f-894e-3919-1083b0180841"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("c9ea40b7-f594-86b7-2d20-b1f9748ecc20"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("e8120980-02b3-9587-8349-9a4b4dc7ebfa"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("eac98a98-b715-6af2-f86a-c7d73629e847"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("fc71dba4-de48-2c0e-1927-96d00fa3a976"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("a0a0a0a0-0a0a-0a0a-0a0a-a0a0a0a0a0a0"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("b0b0b0b0-0b0b-0b0b-0b0b-b0b0b0b0b0b0"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("c0c0c0c0-0c0c-0c0c-0c0c-c0c0c0c0c0c0"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("d0d0d0d0-0d0d-0d0d-0d0d-d0d0d0d0d0d0"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("e0e0e0e0-0e0e-0e0e-0e0e-e0e0e0e0e0e0"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("f0f0f0f0-0f0f-0f0f-0f0f-f0f0f0f0f0f0"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("07b69eec-3d2d-cbc5-6fd9-824559c15c21"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("145982d7-9af7-2943-e9ab-639b3972f84d"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("1dbad435-1f48-7a2e-6353-8c4f135ad3cf"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("27f71bec-8fdb-bd8b-7f43-719e72b591bf"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("2f103dbd-cf76-5f45-b619-a3bc3656e0f8"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("37d70f0e-4b74-7001-95db-139216845aa0"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("515e8274-7978-7bf1-bf92-04e2d60bd874"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("61fbd42f-6d0e-f433-0c79-1bb9186ae253"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("90002ea6-55f1-fdd1-637b-2d315c2e566d"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("ccdc1260-220f-3e1b-2122-d125ce548976"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("d74f385f-999c-f66f-2034-604ff8d5269c"));

            migrationBuilder.InsertData(
                table: "accl_menu_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "MenuId", "PermissionId" },
                values: new object[] { new Guid("30a7c24f-24b3-229d-f2da-62f3a22e47cf"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"), new Guid("a6144ccf-d362-4ed1-cb01-8207890bea3a") });

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "Icon", "Position" },
                values: new object[] { "package", 1 });

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("2f2f2f2f-2f2f-2f2f-2f2f-2f2f2f2f2f2f"),
                column: "Position",
                value: 1);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("3f3f3f3f-3f3f-3f3f-3f3f-3f3f3f3f3f3f"),
                column: "Position",
                value: 2);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("4f4f4f4f-4f4f-4f4f-4f4f-4f4f4f4f4f4f"),
                column: "Position",
                value: 3);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("5f5f5f5f-5f5f-5f5f-5f5f-5f5f5f5f5f5f"),
                column: "Position",
                value: 4);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("6f6f6f6f-6f6f-6f6f-6f6f-6f6f6f6f6f6f"),
                column: "Position",
                value: 5);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"),
                column: "Position",
                value: 6);

            migrationBuilder.InsertData(
                table: "accl_menus",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Icon", "IsActive", "IsDeleted", "Label", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name", "ParentId", "Position", "Route" },
                values: new object[,]
                {
                    { new Guid("1f1f1f1f-1f1f-1f1f-1f1f-1f1f1f1f1f1f"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "users", true, false, "Users", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ADMIN_USERS", new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"), 0, "/admin/users" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "map-pin", true, false, "Locations", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "WAREHOUSE", "LOCATIONS", null, 2, null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "truck", true, false, "Suppliers", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SUPPLIERS", "SUPPLIERS", null, 3, null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "clipboard-list", true, false, "Orders", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ORDERS", null, 4, null },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bar-chart-3", true, false, "Reports", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "REPORTS", "REPORTS", null, 5, null }
                });

            migrationBuilder.InsertData(
                table: "accl_menu_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "MenuId", "PermissionId" },
                values: new object[,]
                {
                    { new Guid("00344a75-ff2e-2874-569e-baaa7c7109d3"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("1f1f1f1f-1f1f-1f1f-1f1f-1f1f1f1f1f1f"), new Guid("bbbfe961-127d-5c27-bd5e-babc2e2b0a6c") },
                    { new Guid("0c8e43cd-a64d-76d1-02a4-88ac8e8c8845"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("1f1f1f1f-1f1f-1f1f-1f1f-1f1f1f1f1f1f"), new Guid("a6144ccf-d362-4ed1-cb01-8207890bea3a") },
                    { new Guid("54d48df9-b29b-8b59-35b0-4554ab7db644"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("f82b18de-ffa1-02ae-09f6-59a09a30ba70") },
                    { new Guid("7416247e-12f3-f95c-959d-5eee316a70bb"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("55555555-5555-5555-5555-555555555555"), new Guid("04bbf5bd-add2-5e8e-2da8-f0e4c24fab23") },
                    { new Guid("b9ec6448-8bb8-f0aa-2156-1d9d8a8b1ce5"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("76b6526d-cff0-4796-1f68-bb2aecd6c9ed") },
                    { new Guid("c12a8a57-1009-a25f-fb60-dbfb582043cd"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("66666666-6666-6666-6666-666666666666"), new Guid("f5d6ff33-b330-1586-1090-119609bb94a8") },
                    { new Guid("cfa93872-5de0-c245-94c3-b1b881f80d5f"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("55555555-5555-5555-5555-555555555555"), new Guid("0f0500ec-ace8-e29c-2cf5-430b2118bf1f") }
                });

            migrationBuilder.InsertData(
                table: "accl_menus",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Icon", "IsActive", "IsDeleted", "Label", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name", "ParentId", "Position", "Route" },
                values: new object[,]
                {
                    { new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "building", true, false, "All Locations", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "LOCATIONS_ALL", new Guid("33333333-3333-3333-3333-333333333333"), 0, "/locations" },
                    { new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "arrow-right-left", true, false, "Stock Transfers", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "LOCATIONS_TRANSFERS", new Guid("33333333-3333-3333-3333-333333333333"), 1, "/locations/transfers" },
                    { new Guid("d2d2d2d2-2d2d-2d2d-2d2d-d2d2d2d2d2d2"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "users", true, false, "Supplier List", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "SUPPLIERS_LIST", new Guid("44444444-4444-4444-4444-444444444444"), 0, "/suppliers" },
                    { new Guid("d3d3d3d3-3d3d-3d3d-3d3d-d3d3d3d3d3d3"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "trending-up", true, false, "Supplier Performance", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "SUPPLIERS_PERFORMANCE", new Guid("44444444-4444-4444-4444-444444444444"), 1, "/suppliers/performance" },
                    { new Guid("e1e1e1e1-1e1e-1e1e-1e1e-e1e1e1e1e1e1"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "shopping-cart", true, false, "Purchase Orders", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PROCUREMENT", "ORDERS_PURCHASE", new Guid("55555555-5555-5555-5555-555555555555"), 0, "/orders/purchase" },
                    { new Guid("e2e2e2e2-2e2e-2e2e-2e2e-e2e2e2e2e2e2"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "receipt", true, false, "Sales Orders", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SALES", "ORDERS_SALES", new Guid("55555555-5555-5555-5555-555555555555"), 1, "/orders/sales" },
                    { new Guid("e3e3e3e3-3e3e-3e3e-3e3e-e3e3e3e3e3e3"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "globe", true, false, "E-Commerce Sync", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ECOMMERCE", "ORDERS_ECOM", new Guid("55555555-5555-5555-5555-555555555555"), 2, "/orders/ecommerce" },
                    { new Guid("f1f1f1f1-1f1f-1f1f-1f1f-f1f1f1f1f1f1"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "file-bar-chart", true, false, "Stock Report", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "REPORTS_STOCK", new Guid("66666666-6666-6666-6666-666666666666"), 0, "/reports/stock" },
                    { new Guid("f2f2f2f2-2f2f-2f2f-2f2f-f2f2f2f2f2f2"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "alert-triangle", true, false, "Expiration Alerts", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "REPORTS_EXPIRATION", new Guid("66666666-6666-6666-6666-666666666666"), 1, "/reports/expiration" },
                    { new Guid("f3f3f3f3-3f3f-3f3f-3f3f-f3f3f3f3f3f3"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "history", true, false, "Movement History", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "REPORTS_MOVEMENTS", new Guid("66666666-6666-6666-6666-666666666666"), 2, "/reports/movements" },
                    { new Guid("f4f4f4f4-4f4f-4f4f-4f4f-f4f4f4f4f4f4"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "trending-up", true, false, "Sales Trends", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "REPORTS_SALES", new Guid("66666666-6666-6666-6666-666666666666"), 3, "/reports/sales-trends" },
                    { new Guid("f5f5f5f5-5f5f-5f5f-5f5f-f5f5f5f5f5f5"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "search", true, false, "Traceability", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "REPORTS_TRACEABILITY", new Guid("66666666-6666-6666-6666-666666666666"), 4, "/reports/traceability" }
                });

            migrationBuilder.InsertData(
                table: "accl_menu_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "MenuId", "PermissionId" },
                values: new object[,]
                {
                    { new Guid("0519010f-24a0-d469-9623-9389456dc15e"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("e2e2e2e2-2e2e-2e2e-2e2e-e2e2e2e2e2e2"), new Guid("0f0500ec-ace8-e29c-2cf5-430b2118bf1f") },
                    { new Guid("1b04d704-9398-dcec-74f4-8ea315a5056c"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("f5f5f5f5-5f5f-5f5f-5f5f-f5f5f5f5f5f5"), new Guid("f5d6ff33-b330-1586-1090-119609bb94a8") },
                    { new Guid("3c517ad2-064b-b36b-c774-7fa2c4212383"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("f4f4f4f4-4f4f-4f4f-4f4f-f4f4f4f4f4f4"), new Guid("f5d6ff33-b330-1586-1090-119609bb94a8") },
                    { new Guid("4fb2a604-9441-d43b-d9f3-ed310c93e290"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1"), new Guid("f82b18de-ffa1-02ae-09f6-59a09a30ba70") },
                    { new Guid("6564aafb-c32f-9241-db36-2b7248794e6d"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"), new Guid("949f5297-cb11-55fe-0577-363d69638021") },
                    { new Guid("6e9b3035-ce9e-73f3-734f-597d8f9c477c"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("f3f3f3f3-3f3f-3f3f-3f3f-f3f3f3f3f3f3"), new Guid("f5d6ff33-b330-1586-1090-119609bb94a8") },
                    { new Guid("84926b1e-6f88-eeea-8bf0-73c86e2a3e47"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d2d2d2d2-2d2d-2d2d-2d2d-d2d2d2d2d2d2"), new Guid("76b6526d-cff0-4796-1f68-bb2aecd6c9ed") },
                    { new Guid("8d08d4b4-c35f-7b8d-9afe-b8322f94ba3d"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("f2f2f2f2-2f2f-2f2f-2f2f-f2f2f2f2f2f2"), new Guid("f5d6ff33-b330-1586-1090-119609bb94a8") },
                    { new Guid("9bc03684-568f-ab12-7bea-fac623ab5190"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d3d3d3d3-3d3d-3d3d-3d3d-d3d3d3d3d3d3"), new Guid("d2402427-5846-8d62-24d8-816c327db0a8") },
                    { new Guid("a1931099-0a4b-295e-1c54-f9016e9ab5f3"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("e1e1e1e1-1e1e-1e1e-1e1e-e1e1e1e1e1e1"), new Guid("04bbf5bd-add2-5e8e-2da8-f0e4c24fab23") },
                    { new Guid("ac1d266b-afe5-85a4-f13b-4f9417636965"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("e3e3e3e3-3e3e-3e3e-3e3e-e3e3e3e3e3e3"), new Guid("8ab5a10d-ba3b-fca2-9ccf-a3e08daa3003") },
                    { new Guid("e8365eb4-db49-bf4a-a604-1b4d1afd3830"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("f1f1f1f1-1f1f-1f1f-1f1f-f1f1f1f1f1f1"), new Guid("f5d6ff33-b330-1586-1090-119609bb94a8") },
                    { new Guid("f1d52b69-e129-58ca-3c56-179948a2960e"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"), new Guid("2bc6429d-df19-9dd1-66f4-c980e29d888c") }
                });
        }
    }
}
