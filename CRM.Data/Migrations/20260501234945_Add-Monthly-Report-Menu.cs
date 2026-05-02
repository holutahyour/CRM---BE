using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthlyReportMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"),
                column: "Position",
                value: 9);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("d0d0d0d0-0d0d-0d0d-0d0d-d0d0d0d0d0d0"),
                column: "Position",
                value: 6);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("e0e0e0e0-0e0e-0e0e-0e0e-e0e0e0e0e0e0"),
                column: "Position",
                value: 7);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("f0f0f0f0-0f0f-0f0f-0f0f-f0f0f0f0f0f0"),
                column: "Position",
                value: 8);

            migrationBuilder.InsertData(
                table: "accl_menus",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "Icon", "IsActive", "IsDeleted", "Label", "LastModifiedBy", "LastModifiedOn", "ModuleCode", "Name", "ParentId", "Position", "Route" },
                values: new object[] { new Guid("d1d1d1d1-1d1d-1d1d-1d1d-d1d1d1d1d1d2"), "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "chart-column", true, false, "Monthly Reports", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CORE", "MONTHLY_REPORTS", null, 5, "/monthly-reports" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("d1d1d1d1-1d1d-1d1d-1d1d-d1d1d1d1d1d2"));

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("a7a7a7a7-7a7a-7a7a-7a7a-a7a7a7a7a7a7"),
                column: "Position",
                value: 8);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("d0d0d0d0-0d0d-0d0d-0d0d-d0d0d0d0d0d0"),
                column: "Position",
                value: 5);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("e0e0e0e0-0e0e-0e0e-0e0e-e0e0e0e0e0e0"),
                column: "Position",
                value: 6);

            migrationBuilder.UpdateData(
                table: "accl_menus",
                keyColumn: "Id",
                keyValue: new Guid("f0f0f0f0-0f0f-0f0f-0f0f-f0f0f0f0f0f0"),
                column: "Position",
                value: 7);
        }
    }
}
