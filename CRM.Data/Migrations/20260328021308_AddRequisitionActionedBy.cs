using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequisitionActionedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ActionedBy",
                table: "Requisitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_ActionedBy",
                table: "Requisitions",
                column: "ActionedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_users_ActionedBy",
                table: "Requisitions",
                column: "ActionedBy",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_users_ActionedBy",
                table: "Requisitions");

            migrationBuilder.DropIndex(
                name: "IX_Requisitions_ActionedBy",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "ActionedBy",
                table: "Requisitions");
        }
    }
}
