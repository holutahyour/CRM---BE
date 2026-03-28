using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddItemRequestActionedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ActionedBy",
                table: "ItemRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemRequests_ActionedBy",
                table: "ItemRequests",
                column: "ActionedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ItemRequests_SubmittedBy",
                table: "ItemRequests",
                column: "SubmittedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemRequests_users_ActionedBy",
                table: "ItemRequests",
                column: "ActionedBy",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemRequests_users_SubmittedBy",
                table: "ItemRequests",
                column: "SubmittedBy",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemRequests_users_ActionedBy",
                table: "ItemRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemRequests_users_SubmittedBy",
                table: "ItemRequests");

            migrationBuilder.DropIndex(
                name: "IX_ItemRequests_ActionedBy",
                table: "ItemRequests");

            migrationBuilder.DropIndex(
                name: "IX_ItemRequests_SubmittedBy",
                table: "ItemRequests");

            migrationBuilder.DropColumn(
                name: "ActionedBy",
                table: "ItemRequests");
        }
    }
}
