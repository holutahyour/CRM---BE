using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("4f4f4f4f-4f4f-4f4f-4f4f-4f4f4f4f4f4f"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("b3b3b3b3-3b3b-3b3b-3b3b-b3b3b3b3b3b3"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("b4b4b4b4-4b4b-4b4b-4b4b-b4b4b4b4b4b4"));

            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("b5b5b5b5-5b5b-5b5b-5b5b-b5b5b5b5b5b5"));

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("d0d0d0d0-0d0d-0d0d-0d0d-d0d0d0d0d0d0"),
                column: "Route",
                value: "/incident-reports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("d0d0d0d0-0d0d-0d0d-0d0d-d0d0d0d0d0d0"),
                column: "Route",
                value: "/incidents");

            migrationBuilder.InsertData(
                table: "accl_menus",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Icon", "IsActive", "IsDeleted", "Label", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name", "ParentId", "Position", "Route" },
                values: new object[,]
                {
                    { new Guid("4f4f4f4f-4f4f-4f4f-4f4f-4f4f4f4f4f4f"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "sliders", true, false, "Tenant Settings", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ADMIN_SETTINGS", new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"), 2, "/admin/settings" },
                    { new Guid("b3b3b3b3-3b3b-3b3b-3b3b-b3b3b3b3b3b3"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "layers", true, false, "Batches & Lots", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INVENTORY_BATCHES", new Guid("22222222-2222-2222-2222-222222222222"), 2, "/inventory/batches" },
                    { new Guid("b4b4b4b4-4b4b-4b4b-4b4b-b4b4b4b4b4b4"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bar-chart", true, false, "Stock Levels", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INVENTORY_STOCK", new Guid("22222222-2222-2222-2222-222222222222"), 3, "/inventory/stock" },
                    { new Guid("b5b5b5b5-5b5b-5b5b-5b5b-b5b5b5b5b5b5"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "scan", true, false, "Barcode Scanner", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "BARCODE", "INVENTORY_SCANNER", new Guid("22222222-2222-2222-2222-222222222222"), 4, "/inventory/scanner" }
                });
        }
    }
}
