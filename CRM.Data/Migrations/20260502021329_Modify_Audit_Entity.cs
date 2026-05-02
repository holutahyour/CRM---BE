using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class Modify_Audit_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MonthlyReports_SubmittedBy",
                table: "MonthlyReports",
                column: "SubmittedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyReports_users_SubmittedBy",
                table: "MonthlyReports",
                column: "SubmittedBy",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyReports_users_SubmittedBy",
                table: "MonthlyReports");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyReports_SubmittedBy",
                table: "MonthlyReports");
        }
    }
}
