using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequisitionSubmittedByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_SubmittedBy",
                table: "Requisitions",
                column: "SubmittedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_users_SubmittedBy",
                table: "Requisitions",
                column: "SubmittedBy",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_users_SubmittedBy",
                table: "Requisitions");

            migrationBuilder.DropIndex(
                name: "IX_Requisitions_SubmittedBy",
                table: "Requisitions");
        }
    }
}
