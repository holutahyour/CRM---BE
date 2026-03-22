using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Access_Control : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "accl_menu_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "MenuId", "PermissionId" },
                values: new object[,]
                {
                    { new Guid("00344a75-ff2e-2874-569e-baaa7c7109d3"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("1f1f1f1f-1f1f-1f1f-1f1f-1f1f1f1f1f1f"), new Guid("bbbfe961-127d-5c27-bd5e-babc2e2b0a6c") },
                    { new Guid("01df28b8-5e81-1e02-b5a7-3c6bf1b457ac"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("b3b3b3b3-3b3b-3b3b-3b3b-b3b3b3b3b3b3"), new Guid("52e3aefd-24df-a0cd-b523-647fccbbd979") },
                    { new Guid("0519010f-24a0-d469-9623-9389456dc15e"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("e2e2e2e2-2e2e-2e2e-2e2e-e2e2e2e2e2e2"), new Guid("0f0500ec-ace8-e29c-2cf5-430b2118bf1f") },
                    { new Guid("138633e2-a265-943f-57e4-647777879796"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("16f14b31-e5b1-816f-ce63-815b91385119") },
                    { new Guid("1b04d704-9398-dcec-74f4-8ea315a5056c"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("f5f5f5f5-5f5f-5f5f-5f5f-f5f5f5f5f5f5"), new Guid("f5d6ff33-b330-1586-1090-119609bb94a8") },
                    { new Guid("2a7023c9-2819-407f-4f15-0eaa824f5399"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("b1b1b1b1-1b1b-1b1b-1b1b-b1b1b1b1b1b1"), new Guid("16f14b31-e5b1-816f-ce63-815b91385119") },
                    { new Guid("3c517ad2-064b-b36b-c774-7fa2c4212383"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("f4f4f4f4-4f4f-4f4f-4f4f-f4f4f4f4f4f4"), new Guid("f5d6ff33-b330-1586-1090-119609bb94a8") },
                    { new Guid("4fb2a604-9441-d43b-d9f3-ed310c93e290"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c1c1c1c1-1c1c-1c1c-1c1c-c1c1c1c1c1c1"), new Guid("f82b18de-ffa1-02ae-09f6-59a09a30ba70") },
                    { new Guid("54d48df9-b29b-8b59-35b0-4554ab7db644"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("f82b18de-ffa1-02ae-09f6-59a09a30ba70") },
                    { new Guid("6564aafb-c32f-9241-db36-2b7248794e6d"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"), new Guid("949f5297-cb11-55fe-0577-363d69638021") },
                    { new Guid("6e9b3035-ce9e-73f3-734f-597d8f9c477c"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("f3f3f3f3-3f3f-3f3f-3f3f-f3f3f3f3f3f3"), new Guid("f5d6ff33-b330-1586-1090-119609bb94a8") },
                    { new Guid("7416247e-12f3-f95c-959d-5eee316a70bb"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("55555555-5555-5555-5555-555555555555"), new Guid("04bbf5bd-add2-5e8e-2da8-f0e4c24fab23") },
                    { new Guid("7821143d-c371-6419-52bd-9234da82ac98"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("5f5f5f5f-5f5f-5f5f-5f5f-5f5f5f5f5f5f"), new Guid("250563d5-86c5-2fc4-e574-8189d0c03b3b") },
                    { new Guid("84503eae-cc09-fd42-af20-de83a0f85360"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("b2b2b2b2-2b2b-2b2b-2b2b-b2b2b2b2b2b2"), new Guid("0febd349-7a8c-da4e-6c49-c59d0b2caf39") },
                    { new Guid("84926b1e-6f88-eeea-8bf0-73c86e2a3e47"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d2d2d2d2-2d2d-2d2d-2d2d-d2d2d2d2d2d2"), new Guid("76b6526d-cff0-4796-1f68-bb2aecd6c9ed") },
                    { new Guid("8d08d4b4-c35f-7b8d-9afe-b8322f94ba3d"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("f2f2f2f2-2f2f-2f2f-2f2f-f2f2f2f2f2f2"), new Guid("f5d6ff33-b330-1586-1090-119609bb94a8") },
                    { new Guid("8f4a871e-7eea-6c45-3b6c-b26be969fe8a"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("2f2f2f2f-2f2f-2f2f-2f2f-2f2f2f2f2f2f"), new Guid("e0a63f57-fb0b-4f4e-2d69-a523aa73f746") },
                    { new Guid("a1931099-0a4b-295e-1c54-f9016e9ab5f3"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("e1e1e1e1-1e1e-1e1e-1e1e-e1e1e1e1e1e1"), new Guid("04bbf5bd-add2-5e8e-2da8-f0e4c24fab23") },
                    { new Guid("b3334095-9d48-330c-9fe7-1a76e8fa67eb"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("4f4f4f4f-4f4f-4f4f-4f4f-4f4f4f4f4f4f"), new Guid("dd6be378-a8bb-c2de-632e-1e8a1ad24913") },
                    { new Guid("b9ec6448-8bb8-f0aa-2156-1d9d8a8b1ce5"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("76b6526d-cff0-4796-1f68-bb2aecd6c9ed") },
                    { new Guid("c12a8a57-1009-a25f-fb60-dbfb582043cd"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("66666666-6666-6666-6666-666666666666"), new Guid("f5d6ff33-b330-1586-1090-119609bb94a8") },
                    { new Guid("cfa93872-5de0-c245-94c3-b1b881f80d5f"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("55555555-5555-5555-5555-555555555555"), new Guid("0f0500ec-ace8-e29c-2cf5-430b2118bf1f") },
                    { new Guid("e8365eb4-db49-bf4a-a604-1b4d1afd3830"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("f1f1f1f1-1f1f-1f1f-1f1f-f1f1f1f1f1f1"), new Guid("f5d6ff33-b330-1586-1090-119609bb94a8") },
                    { new Guid("ee7c0d86-942a-dc8d-d75f-b34475bcb0d3"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("3f3f3f3f-3f3f-3f3f-3f3f-3f3f3f3f3f3f"), new Guid("3d8a7d5e-b379-dd8e-431c-314b7a6040ae") }
                });

            migrationBuilder.InsertData(
                table: "accl_menus",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Icon", "IsActive", "IsDeleted", "Label", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name", "ParentId", "Position", "Route" },
                values: new object[] { new Guid("6f6f6f6f-6f6f-6f6f-6f6f-6f6f6f6f6f6f"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "layout-grid", true, false, "Menu Management", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ADMIN_MENUS", new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"), 5, "/admin/menus" });

            migrationBuilder.InsertData(
                table: "accl_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Description", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name" },
                values: new object[,]
                {
                    { new Guid("12d040d1-56fa-8c03-95d7-983522eca589"), "inventory.stock.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "INVENTORY", "View Stock" },
                    { new Guid("26d249f1-7257-c681-8b3a-63535b1414f2"), "admin.menus.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "View Menus" },
                    { new Guid("2bc6429d-df19-9dd1-66f4-c980e29d888c"), "inventory.transfers.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "INVENTORY", "View Transfers" },
                    { new Guid("6ff25cfe-2e1f-a30f-f183-8794a4ece371"), "admin.roles.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "View Roles" },
                    { new Guid("7d34b8d7-1cea-b7c4-235e-2a5f64da143d"), "admin.modules.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "View Modules" },
                    { new Guid("8ab5a10d-ba3b-fca2-9ccf-a3e08daa3003"), "orders.ecommerce.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SALES", "Manage Ecommerce" },
                    { new Guid("9fe16d16-8634-6c0b-453a-32df8bb4beab"), "inventory.scanner.use", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "INVENTORY", "Use Scanner" },
                    { new Guid("a6144ccf-d362-4ed1-cb01-8207890bea3a"), "admin.users.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "View Users" },
                    { new Guid("aeab1df7-0060-894b-1cce-0e35c5baaec9"), "admin.settings.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "View Settings" },
                    { new Guid("d16e5975-dab2-15eb-aeb4-6027aea8eae6"), "admin.menus.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "Manage Menus" },
                    { new Guid("d2402427-5846-8d62-24d8-816c327db0a8"), "suppliers.performance.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SUPPLIERS", "View Performance" }
                });

            migrationBuilder.InsertData(
                table: "accl_menu_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "MenuId", "PermissionId" },
                values: new object[,]
                {
                    { new Guid("0c8e43cd-a64d-76d1-02a4-88ac8e8c8845"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("1f1f1f1f-1f1f-1f1f-1f1f-1f1f1f1f1f1f"), new Guid("a6144ccf-d362-4ed1-cb01-8207890bea3a") },
                    { new Guid("25f4f475-3d13-1863-b93d-aa7c4b7a34a1"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("2f2f2f2f-2f2f-2f2f-2f2f-2f2f2f2f2f2f"), new Guid("6ff25cfe-2e1f-a30f-f183-8794a4ece371") },
                    { new Guid("30a7c24f-24b3-229d-f2da-62f3a22e47cf"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"), new Guid("a6144ccf-d362-4ed1-cb01-8207890bea3a") },
                    { new Guid("40798021-fd96-6a82-a31b-777d5c47f94f"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("3f3f3f3f-3f3f-3f3f-3f3f-3f3f3f3f3f3f"), new Guid("7d34b8d7-1cea-b7c4-235e-2a5f64da143d") },
                    { new Guid("426ed3f4-a800-954f-d24a-f4808c03f9b7"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("b4b4b4b4-4b4b-4b4b-4b4b-b4b4b4b4b4b4"), new Guid("12d040d1-56fa-8c03-95d7-983522eca589") },
                    { new Guid("52087f6d-eba0-2192-b72c-01016b67f673"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"), new Guid("6ff25cfe-2e1f-a30f-f183-8794a4ece371") },
                    { new Guid("5e998fb2-c6c2-c181-f22b-33b20f8606a6"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("6f6f6f6f-6f6f-6f6f-6f6f-6f6f6f6f6f6f"), new Guid("d16e5975-dab2-15eb-aeb4-6027aea8eae6") },
                    { new Guid("9bc03684-568f-ab12-7bea-fac623ab5190"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d3d3d3d3-3d3d-3d3d-3d3d-d3d3d3d3d3d3"), new Guid("d2402427-5846-8d62-24d8-816c327db0a8") },
                    { new Guid("ac1d266b-afe5-85a4-f13b-4f9417636965"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("e3e3e3e3-3e3e-3e3e-3e3e-e3e3e3e3e3e3"), new Guid("8ab5a10d-ba3b-fca2-9ccf-a3e08daa3003") },
                    { new Guid("ca19857b-d0e6-c2f4-f27d-7e9de9d6ce93"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("6f6f6f6f-6f6f-6f6f-6f6f-6f6f6f6f6f6f"), new Guid("26d249f1-7257-c681-8b3a-63535b1414f2") },
                    { new Guid("e3abb29b-1c11-c123-8394-baae23cbb8d1"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("b5b5b5b5-5b5b-5b5b-5b5b-b5b5b5b5b5b5"), new Guid("9fe16d16-8634-6c0b-453a-32df8bb4beab") },
                    { new Guid("e84cba74-0271-9dfc-27b5-047f532da84b"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("4f4f4f4f-4f4f-4f4f-4f4f-4f4f4f4f4f4f"), new Guid("aeab1df7-0060-894b-1cce-0e35c5baaec9") },
                    { new Guid("f1d52b69-e129-58ca-3c56-179948a2960e"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c2c2c2c2-2c2c-2c2c-2c2c-c2c2c2c2c2c2"), new Guid("2bc6429d-df19-9dd1-66f4-c980e29d888c") }
                });

            migrationBuilder.InsertData(
                table: "accl_role_permissions",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsDeleted", "LastModifiedBy", "LastModifiedOn", "PermissionId", "RoleId", "TenantId" },
                values: new object[,]
                {
                    { new Guid("21eb0740-68a0-2022-be10-174036a55555"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("12d040d1-56fa-8c03-95d7-983522eca589"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("40638e77-7340-4345-5776-0d23fbdb1f44"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("9fe16d16-8634-6c0b-453a-32df8bb4beab"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("80522781-32da-d24e-8b28-3fe4f2a6abf6"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("8ab5a10d-ba3b-fca2-9ccf-a3e08daa3003"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("89860ce1-5556-3468-012a-a23c1a468488"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d2402427-5846-8d62-24d8-816c327db0a8"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("89cd3c32-66f4-6b83-19d2-8ee78c7235f2"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d16e5975-dab2-15eb-aeb4-6027aea8eae6"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("8bad99b6-56b5-2664-a7f1-679bb87678e4"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("7d34b8d7-1cea-b7c4-235e-2a5f64da143d"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("b25181c5-d124-4c73-3616-106d5afcd80f"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("a6144ccf-d362-4ed1-cb01-8207890bea3a"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("c5bf0aa4-de54-a8d9-8ae3-aef574645366"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("2bc6429d-df19-9dd1-66f4-c980e29d888c"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("c7c63106-0ce3-7652-e203-52c00ebfa237"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("6ff25cfe-2e1f-a30f-f183-8794a4ece371"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("e9d39772-088c-d4a5-4cf7-4a70a227c966"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("26d249f1-7257-c681-8b3a-63535b1414f2"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("ea1d5d81-bc69-7fb9-aa8c-e7db27d7ed57"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("aeab1df7-0060-894b-1cce-0e35c5baaec9"), new Guid("f0f0f0f0-f0f0-f0f0-f0f0-f0f0f0f0f0f0"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("00344a75-ff2e-2874-569e-baaa7c7109d3"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("01df28b8-5e81-1e02-b5a7-3c6bf1b457ac"));

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
                keyValue: new Guid("138633e2-a265-943f-57e4-647777879796"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("1b04d704-9398-dcec-74f4-8ea315a5056c"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("25f4f475-3d13-1863-b93d-aa7c4b7a34a1"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("2a7023c9-2819-407f-4f15-0eaa824f5399"));

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
                keyValue: new Guid("40798021-fd96-6a82-a31b-777d5c47f94f"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("426ed3f4-a800-954f-d24a-f4808c03f9b7"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("4fb2a604-9441-d43b-d9f3-ed310c93e290"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("52087f6d-eba0-2192-b72c-01016b67f673"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("54d48df9-b29b-8b59-35b0-4554ab7db644"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("5e998fb2-c6c2-c181-f22b-33b20f8606a6"));

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
                keyValue: new Guid("7821143d-c371-6419-52bd-9234da82ac98"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("84503eae-cc09-fd42-af20-de83a0f85360"));

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
                keyValue: new Guid("8f4a871e-7eea-6c45-3b6c-b26be969fe8a"));

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
                keyValue: new Guid("b3334095-9d48-330c-9fe7-1a76e8fa67eb"));

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
                keyValue: new Guid("ca19857b-d0e6-c2f4-f27d-7e9de9d6ce93"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("cfa93872-5de0-c245-94c3-b1b881f80d5f"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("e3abb29b-1c11-c123-8394-baae23cbb8d1"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("e8365eb4-db49-bf4a-a604-1b4d1afd3830"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("e84cba74-0271-9dfc-27b5-047f532da84b"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("ee7c0d86-942a-dc8d-d75f-b34475bcb0d3"));

            migrationBuilder.DeleteData(
                table: "accl_menu_permissions",
                keyColumn: "Id",
                keyValue: new Guid("f1d52b69-e129-58ca-3c56-179948a2960e"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("21eb0740-68a0-2022-be10-174036a55555"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("40638e77-7340-4345-5776-0d23fbdb1f44"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("80522781-32da-d24e-8b28-3fe4f2a6abf6"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("89860ce1-5556-3468-012a-a23c1a468488"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("89cd3c32-66f4-6b83-19d2-8ee78c7235f2"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("8bad99b6-56b5-2664-a7f1-679bb87678e4"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("b25181c5-d124-4c73-3616-106d5afcd80f"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("c5bf0aa4-de54-a8d9-8ae3-aef574645366"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("c7c63106-0ce3-7652-e203-52c00ebfa237"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("e9d39772-088c-d4a5-4cf7-4a70a227c966"));

            migrationBuilder.DeleteData(
                table: "accl_role_permissions",
                keyColumn: "Id",
                keyValue: new Guid("ea1d5d81-bc69-7fb9-aa8c-e7db27d7ed57"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("6f6f6f6f-6f6f-6f6f-6f6f-6f6f6f6f6f6f"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("12d040d1-56fa-8c03-95d7-983522eca589"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("26d249f1-7257-c681-8b3a-63535b1414f2"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("2bc6429d-df19-9dd1-66f4-c980e29d888c"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("6ff25cfe-2e1f-a30f-f183-8794a4ece371"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("7d34b8d7-1cea-b7c4-235e-2a5f64da143d"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("8ab5a10d-ba3b-fca2-9ccf-a3e08daa3003"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("9fe16d16-8634-6c0b-453a-32df8bb4beab"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("a6144ccf-d362-4ed1-cb01-8207890bea3a"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("aeab1df7-0060-894b-1cce-0e35c5baaec9"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("d16e5975-dab2-15eb-aeb4-6027aea8eae6"));

            migrationBuilder.DeleteData(
                table: "accl_permissions",
                keyColumn: "Id",
                keyValue: new Guid("d2402427-5846-8d62-24d8-816c327db0a8"));
        }
    }
}
