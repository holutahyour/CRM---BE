using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityContextFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "req_activities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "req_activities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "req_activities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_req_activities_DepartmentId",
                table: "req_activities",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_req_activities_org_departments_DepartmentId",
                table: "req_activities",
                column: "DepartmentId",
                principalTable: "org_departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_req_activities_org_departments_DepartmentId",
                table: "req_activities");

            migrationBuilder.DropIndex(
                name: "IX_req_activities_DepartmentId",
                table: "req_activities");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "req_activities");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "req_activities");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "req_activities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
