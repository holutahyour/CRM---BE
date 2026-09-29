using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationsProcessing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ops_order_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CustomerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Products = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ActivitiesRequired = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    VolumeRequired = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DeliveryLocation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProductBatchNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ops_order_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ops_products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Upc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RawMaterialId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProcessingDuration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ops_products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ops_production_batches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RawMaterialBatchId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BatchCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProductNames = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    QuantityUnit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    QuantityNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LeadTime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WorkCenters = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Operators = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaskDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    QualityChecks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DateSent = table.Column<DateOnly>(type: "date", nullable: true),
                    QuantitySent = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LogisticsPersonnel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DeliveryStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OnTimeDeliveryPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ops_production_batches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ops_production_batches_ops_order_requests_OrderRequestId",
                        column: x => x.OrderRequestId,
                        principalTable: "ops_order_requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ops_production_batches_ops_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ops_products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ops_yield_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ProduceItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductionBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Shift = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    InputQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InputUnit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    InputWeightKg = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    CutWeightKg = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    DehydratedWeightKg = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    GrindWeightKg = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    SecondGrindWeightKg = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    WasteKg = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ops_yield_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ops_yield_entries_items_ProduceItemId",
                        column: x => x.ProduceItemId,
                        principalTable: "items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ops_yield_entries_ops_production_batches_ProductionBatchId",
                        column: x => x.ProductionBatchId,
                        principalTable: "ops_production_batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ops_order_requests_TenantId_RequestDate",
                table: "ops_order_requests",
                columns: new[] { "TenantId", "RequestDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ops_production_batches_OrderRequestId",
                table: "ops_production_batches",
                column: "OrderRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ops_production_batches_ProductId",
                table: "ops_production_batches",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ops_production_batches_TenantId_StartDate",
                table: "ops_production_batches",
                columns: new[] { "TenantId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ops_products_TenantId_Name",
                table: "ops_products",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_ops_yield_entries_ProduceItemId",
                table: "ops_yield_entries",
                column: "ProduceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ops_yield_entries_ProductionBatchId",
                table: "ops_yield_entries",
                column: "ProductionBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ops_yield_entries_TenantId_Date",
                table: "ops_yield_entries",
                columns: new[] { "TenantId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ops_yield_entries");

            migrationBuilder.DropTable(
                name: "ops_production_batches");

            migrationBuilder.DropTable(
                name: "ops_order_requests");

            migrationBuilder.DropTable(
                name: "ops_products");
        }
    }
}
