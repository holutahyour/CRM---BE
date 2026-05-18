using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_DepartmentId",
                table: "users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_req_incidents_ReportedBy",
                table: "req_incidents",
                column: "ReportedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_req_incidents_users_ReportedBy",
                table: "req_incidents",
                column: "ReportedBy",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_org_departments_DepartmentId",
                table: "users",
                column: "DepartmentId",
                principalTable: "org_departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_req_incidents_users_ReportedBy",
                table: "req_incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_users_org_departments_DepartmentId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_DepartmentId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_req_incidents_ReportedBy",
                table: "req_incidents");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "users");
        }
    }
}
