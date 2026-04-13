using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddItemLocationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemLocation",
                table: "items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "items",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_items_LocationId",
                table: "items",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_items_invtry_locations_LocationId",
                table: "items",
                column: "LocationId",
                principalTable: "invtry_locations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_invtry_locations_LocationId",
                table: "items");

            migrationBuilder.DropIndex(
                name: "IX_items_LocationId",
                table: "items");

            migrationBuilder.DropColumn(
                name: "ItemLocation",
                table: "items");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "items");
        }
    }
}
