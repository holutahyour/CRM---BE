using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Requisitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_Categories_CategoryId",
                table: "items");

            migrationBuilder.DropForeignKey(
                name: "FK_items_Vendors_VendorId",
                table: "items");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuPermissions_Menus_MenuId1",
                table: "MenuPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuPermissions_Permissions_PermissionId1",
                table: "MenuPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Menus_ParentId",
                table: "Menus");

            migrationBuilder.DropForeignKey(
                name: "FK_Modules_ModuleCategories_CategoryId",
                table: "Modules");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItems_Locations_LocationId1",
                table: "PurchaseOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId1",
                table: "PurchaseOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItems_items_ItemId1",
                table: "PurchaseOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Vendors_VendorId1",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrderItems_Batches_BatchId1",
                table: "SalesOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrderItems_SalesOrders_SalesOrderId1",
                table: "SalesOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrderItems_items_ItemId1",
                table: "SalesOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantModules_Modules_ModuleId",
                table: "TenantModules");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantModules_tenants_TenantId",
                table: "TenantModules");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_users_UserId",
                table: "UserRoles");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "InventoryTransactions");

            migrationBuilder.DropTable(
                name: "ItemLocations");

            migrationBuilder.DropTable(
                name: "Batches");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vendors",
                table: "Vendors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantModules",
                table: "TenantModules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalesOrders",
                table: "SalesOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalesOrderItems",
                table: "SalesOrderItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseOrders",
                table: "PurchaseOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseOrderItems",
                table: "PurchaseOrderItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Modules",
                table: "Modules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModuleCategories",
                table: "ModuleCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Menus",
                table: "Menus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuPermissions",
                table: "MenuPermissions");

            migrationBuilder.RenameTable(
                name: "Vendors",
                newName: "ordr_vendors");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "accl_user_roles");

            migrationBuilder.RenameTable(
                name: "TenantModules",
                newName: "sys_tenant_modules");

            migrationBuilder.RenameTable(
                name: "SalesOrders",
                newName: "ordr_sales_orders");

            migrationBuilder.RenameTable(
                name: "SalesOrderItems",
                newName: "ordr_sales_order_items");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "accl_roles");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                newName: "accl_role_permissions");

            migrationBuilder.RenameTable(
                name: "PurchaseOrders",
                newName: "ordr_purchase_orders");

            migrationBuilder.RenameTable(
                name: "PurchaseOrderItems",
                newName: "ordr_purchase_order_items");

            migrationBuilder.RenameTable(
                name: "Permissions",
                newName: "accl_permissions");

            migrationBuilder.RenameTable(
                name: "Modules",
                newName: "sys_modules");

            migrationBuilder.RenameTable(
                name: "ModuleCategories",
                newName: "sys_module_categories");

            migrationBuilder.RenameTable(
                name: "Menus",
                newName: "accl_menus");

            migrationBuilder.RenameTable(
                name: "MenuPermissions",
                newName: "accl_menu_permissions");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_UserId",
                table: "accl_user_roles",
                newName: "IX_accl_user_roles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_RoleId",
                table: "accl_user_roles",
                newName: "IX_accl_user_roles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantModules_TenantId",
                table: "sys_tenant_modules",
                newName: "IX_sys_tenant_modules_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantModules_ModuleId",
                table: "sys_tenant_modules",
                newName: "IX_sys_tenant_modules_ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesOrderItems_SalesOrderId1",
                table: "ordr_sales_order_items",
                newName: "IX_ordr_sales_order_items_SalesOrderId1");

            migrationBuilder.RenameIndex(
                name: "IX_SalesOrderItems_ItemId1",
                table: "ordr_sales_order_items",
                newName: "IX_ordr_sales_order_items_ItemId1");

            migrationBuilder.RenameIndex(
                name: "IX_SalesOrderItems_BatchId1",
                table: "ordr_sales_order_items",
                newName: "IX_ordr_sales_order_items_BatchId1");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissions_RoleId",
                table: "accl_role_permissions",
                newName: "IX_accl_role_permissions_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "accl_role_permissions",
                newName: "IX_accl_role_permissions_PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrders_VendorId1",
                table: "ordr_purchase_orders",
                newName: "IX_ordr_purchase_orders_VendorId1");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrderItems_PurchaseOrderId1",
                table: "ordr_purchase_order_items",
                newName: "IX_ordr_purchase_order_items_PurchaseOrderId1");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrderItems_LocationId1",
                table: "ordr_purchase_order_items",
                newName: "IX_ordr_purchase_order_items_LocationId1");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrderItems_ItemId1",
                table: "ordr_purchase_order_items",
                newName: "IX_ordr_purchase_order_items_ItemId1");

            migrationBuilder.RenameIndex(
                name: "IX_Modules_CategoryId",
                table: "sys_modules",
                newName: "IX_sys_modules_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Menus_ParentId",
                table: "accl_menus",
                newName: "IX_accl_menus_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuPermissions_PermissionId1",
                table: "accl_menu_permissions",
                newName: "IX_accl_menu_permissions_PermissionId1");

            migrationBuilder.RenameIndex(
                name: "IX_MenuPermissions_MenuId1",
                table: "accl_menu_permissions",
                newName: "IX_accl_menu_permissions_MenuId1");

            migrationBuilder.AddColumn<int>(
                name: "AccessLevel",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Onboarded",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ordr_vendors",
                table: "ordr_vendors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accl_user_roles",
                table: "accl_user_roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sys_tenant_modules",
                table: "sys_tenant_modules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ordr_sales_orders",
                table: "ordr_sales_orders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ordr_sales_order_items",
                table: "ordr_sales_order_items",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accl_roles",
                table: "accl_roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accl_role_permissions",
                table: "accl_role_permissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ordr_purchase_orders",
                table: "ordr_purchase_orders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ordr_purchase_order_items",
                table: "ordr_purchase_order_items",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accl_permissions",
                table: "accl_permissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sys_modules",
                table: "sys_modules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sys_module_categories",
                table: "sys_module_categories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accl_menus",
                table: "accl_menus",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accl_menu_permissions",
                table: "accl_menu_permissions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "invtry_batches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InitialQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SupplierBatchId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_invtry_batches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_invtry_batches_items_ItemId1",
                        column: x => x.ItemId1,
                        principalTable: "items",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "invtry_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_invtry_categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_invtry_categories_invtry_categories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "invtry_categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "invtry_locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Capacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TemperatureControl = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invtry_locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "invtry_inventory_transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    BatchId = table.Column<int>(type: "int", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ItemId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BatchId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LocationId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_invtry_inventory_transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_invtry_inventory_transactions_invtry_batches_BatchId1",
                        column: x => x.BatchId1,
                        principalTable: "invtry_batches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_invtry_inventory_transactions_invtry_locations_LocationId1",
                        column: x => x.LocationId1,
                        principalTable: "invtry_locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_invtry_inventory_transactions_items_ItemId1",
                        column: x => x.ItemId1,
                        principalTable: "items",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "invtry_item_locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reserved = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Damaged = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItemId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LocationId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_invtry_item_locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_invtry_item_locations_invtry_locations_LocationId1",
                        column: x => x.LocationId1,
                        principalTable: "invtry_locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_invtry_item_locations_items_ItemId1",
                        column: x => x.ItemId1,
                        principalTable: "items",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_invtry_batches_ItemId1",
                table: "invtry_batches",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_invtry_categories_ParentId",
                table: "invtry_categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_invtry_inventory_transactions_BatchId1",
                table: "invtry_inventory_transactions",
                column: "BatchId1");

            migrationBuilder.CreateIndex(
                name: "IX_invtry_inventory_transactions_ItemId1",
                table: "invtry_inventory_transactions",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_invtry_inventory_transactions_LocationId1",
                table: "invtry_inventory_transactions",
                column: "LocationId1");

            migrationBuilder.CreateIndex(
                name: "IX_invtry_item_locations_ItemId1",
                table: "invtry_item_locations",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_invtry_item_locations_LocationId1",
                table: "invtry_item_locations",
                column: "LocationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_accl_menu_permissions_accl_menus_MenuId1",
                table: "accl_menu_permissions",
                column: "MenuId1",
                principalTable: "accl_menus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_accl_menu_permissions_accl_permissions_PermissionId1",
                table: "accl_menu_permissions",
                column: "PermissionId1",
                principalTable: "accl_permissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_accl_menus_accl_menus_ParentId",
                table: "accl_menus",
                column: "ParentId",
                principalTable: "accl_menus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_accl_role_permissions_accl_permissions_PermissionId",
                table: "accl_role_permissions",
                column: "PermissionId",
                principalTable: "accl_permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_accl_role_permissions_accl_roles_RoleId",
                table: "accl_role_permissions",
                column: "RoleId",
                principalTable: "accl_roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_accl_user_roles_accl_roles_RoleId",
                table: "accl_user_roles",
                column: "RoleId",
                principalTable: "accl_roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_accl_user_roles_users_UserId",
                table: "accl_user_roles",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_items_invtry_categories_CategoryId",
                table: "items",
                column: "CategoryId",
                principalTable: "invtry_categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_items_ordr_vendors_VendorId",
                table: "items",
                column: "VendorId",
                principalTable: "ordr_vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ordr_purchase_order_items_invtry_locations_LocationId1",
                table: "ordr_purchase_order_items",
                column: "LocationId1",
                principalTable: "invtry_locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ordr_purchase_order_items_items_ItemId1",
                table: "ordr_purchase_order_items",
                column: "ItemId1",
                principalTable: "items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ordr_purchase_order_items_ordr_purchase_orders_PurchaseOrderId1",
                table: "ordr_purchase_order_items",
                column: "PurchaseOrderId1",
                principalTable: "ordr_purchase_orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ordr_purchase_orders_ordr_vendors_VendorId1",
                table: "ordr_purchase_orders",
                column: "VendorId1",
                principalTable: "ordr_vendors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ordr_sales_order_items_invtry_batches_BatchId1",
                table: "ordr_sales_order_items",
                column: "BatchId1",
                principalTable: "invtry_batches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ordr_sales_order_items_items_ItemId1",
                table: "ordr_sales_order_items",
                column: "ItemId1",
                principalTable: "items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ordr_sales_order_items_ordr_sales_orders_SalesOrderId1",
                table: "ordr_sales_order_items",
                column: "SalesOrderId1",
                principalTable: "ordr_sales_orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sys_modules_sys_module_categories_CategoryId",
                table: "sys_modules",
                column: "CategoryId",
                principalTable: "sys_module_categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sys_tenant_modules_sys_modules_ModuleId",
                table: "sys_tenant_modules",
                column: "ModuleId",
                principalTable: "sys_modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sys_tenant_modules_tenants_TenantId",
                table: "sys_tenant_modules",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accl_menu_permissions_accl_menus_MenuId1",
                table: "accl_menu_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_accl_menu_permissions_accl_permissions_PermissionId1",
                table: "accl_menu_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_accl_menus_accl_menus_ParentId",
                table: "accl_menus");

            migrationBuilder.DropForeignKey(
                name: "FK_accl_role_permissions_accl_permissions_PermissionId",
                table: "accl_role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_accl_role_permissions_accl_roles_RoleId",
                table: "accl_role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_accl_user_roles_accl_roles_RoleId",
                table: "accl_user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_accl_user_roles_users_UserId",
                table: "accl_user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_items_invtry_categories_CategoryId",
                table: "items");

            migrationBuilder.DropForeignKey(
                name: "FK_items_ordr_vendors_VendorId",
                table: "items");

            migrationBuilder.DropForeignKey(
                name: "FK_ordr_purchase_order_items_invtry_locations_LocationId1",
                table: "ordr_purchase_order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_ordr_purchase_order_items_items_ItemId1",
                table: "ordr_purchase_order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_ordr_purchase_order_items_ordr_purchase_orders_PurchaseOrderId1",
                table: "ordr_purchase_order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_ordr_purchase_orders_ordr_vendors_VendorId1",
                table: "ordr_purchase_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_ordr_sales_order_items_invtry_batches_BatchId1",
                table: "ordr_sales_order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_ordr_sales_order_items_items_ItemId1",
                table: "ordr_sales_order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_ordr_sales_order_items_ordr_sales_orders_SalesOrderId1",
                table: "ordr_sales_order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_sys_modules_sys_module_categories_CategoryId",
                table: "sys_modules");

            migrationBuilder.DropForeignKey(
                name: "FK_sys_tenant_modules_sys_modules_ModuleId",
                table: "sys_tenant_modules");

            migrationBuilder.DropForeignKey(
                name: "FK_sys_tenant_modules_tenants_TenantId",
                table: "sys_tenant_modules");

            migrationBuilder.DropTable(
                name: "invtry_categories");

            migrationBuilder.DropTable(
                name: "invtry_inventory_transactions");

            migrationBuilder.DropTable(
                name: "invtry_item_locations");

            migrationBuilder.DropTable(
                name: "invtry_batches");

            migrationBuilder.DropTable(
                name: "invtry_locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sys_tenant_modules",
                table: "sys_tenant_modules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sys_modules",
                table: "sys_modules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sys_module_categories",
                table: "sys_module_categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ordr_vendors",
                table: "ordr_vendors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ordr_sales_orders",
                table: "ordr_sales_orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ordr_sales_order_items",
                table: "ordr_sales_order_items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ordr_purchase_orders",
                table: "ordr_purchase_orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ordr_purchase_order_items",
                table: "ordr_purchase_order_items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accl_user_roles",
                table: "accl_user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accl_roles",
                table: "accl_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accl_role_permissions",
                table: "accl_role_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accl_permissions",
                table: "accl_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accl_menus",
                table: "accl_menus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accl_menu_permissions",
                table: "accl_menu_permissions");

            migrationBuilder.DropColumn(
                name: "AccessLevel",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Onboarded",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "users");

            migrationBuilder.RenameTable(
                name: "sys_tenant_modules",
                newName: "TenantModules");

            migrationBuilder.RenameTable(
                name: "sys_modules",
                newName: "Modules");

            migrationBuilder.RenameTable(
                name: "sys_module_categories",
                newName: "ModuleCategories");

            migrationBuilder.RenameTable(
                name: "ordr_vendors",
                newName: "Vendors");

            migrationBuilder.RenameTable(
                name: "ordr_sales_orders",
                newName: "SalesOrders");

            migrationBuilder.RenameTable(
                name: "ordr_sales_order_items",
                newName: "SalesOrderItems");

            migrationBuilder.RenameTable(
                name: "ordr_purchase_orders",
                newName: "PurchaseOrders");

            migrationBuilder.RenameTable(
                name: "ordr_purchase_order_items",
                newName: "PurchaseOrderItems");

            migrationBuilder.RenameTable(
                name: "accl_user_roles",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "accl_roles",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "accl_role_permissions",
                newName: "RolePermissions");

            migrationBuilder.RenameTable(
                name: "accl_permissions",
                newName: "Permissions");

            migrationBuilder.RenameTable(
                name: "accl_menus",
                newName: "Menus");

            migrationBuilder.RenameTable(
                name: "accl_menu_permissions",
                newName: "MenuPermissions");

            migrationBuilder.RenameIndex(
                name: "IX_sys_tenant_modules_TenantId",
                table: "TenantModules",
                newName: "IX_TenantModules_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_sys_tenant_modules_ModuleId",
                table: "TenantModules",
                newName: "IX_TenantModules_ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_sys_modules_CategoryId",
                table: "Modules",
                newName: "IX_Modules_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ordr_sales_order_items_SalesOrderId1",
                table: "SalesOrderItems",
                newName: "IX_SalesOrderItems_SalesOrderId1");

            migrationBuilder.RenameIndex(
                name: "IX_ordr_sales_order_items_ItemId1",
                table: "SalesOrderItems",
                newName: "IX_SalesOrderItems_ItemId1");

            migrationBuilder.RenameIndex(
                name: "IX_ordr_sales_order_items_BatchId1",
                table: "SalesOrderItems",
                newName: "IX_SalesOrderItems_BatchId1");

            migrationBuilder.RenameIndex(
                name: "IX_ordr_purchase_orders_VendorId1",
                table: "PurchaseOrders",
                newName: "IX_PurchaseOrders_VendorId1");

            migrationBuilder.RenameIndex(
                name: "IX_ordr_purchase_order_items_PurchaseOrderId1",
                table: "PurchaseOrderItems",
                newName: "IX_PurchaseOrderItems_PurchaseOrderId1");

            migrationBuilder.RenameIndex(
                name: "IX_ordr_purchase_order_items_LocationId1",
                table: "PurchaseOrderItems",
                newName: "IX_PurchaseOrderItems_LocationId1");

            migrationBuilder.RenameIndex(
                name: "IX_ordr_purchase_order_items_ItemId1",
                table: "PurchaseOrderItems",
                newName: "IX_PurchaseOrderItems_ItemId1");

            migrationBuilder.RenameIndex(
                name: "IX_accl_user_roles_UserId",
                table: "UserRoles",
                newName: "IX_UserRoles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_accl_user_roles_RoleId",
                table: "UserRoles",
                newName: "IX_UserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_accl_role_permissions_RoleId",
                table: "RolePermissions",
                newName: "IX_RolePermissions_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_accl_role_permissions_PermissionId",
                table: "RolePermissions",
                newName: "IX_RolePermissions_PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_accl_menus_ParentId",
                table: "Menus",
                newName: "IX_Menus_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_accl_menu_permissions_PermissionId1",
                table: "MenuPermissions",
                newName: "IX_MenuPermissions_PermissionId1");

            migrationBuilder.RenameIndex(
                name: "IX_accl_menu_permissions_MenuId1",
                table: "MenuPermissions",
                newName: "IX_MenuPermissions_MenuId1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantModules",
                table: "TenantModules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Modules",
                table: "Modules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModuleCategories",
                table: "ModuleCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vendors",
                table: "Vendors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalesOrders",
                table: "SalesOrders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalesOrderItems",
                table: "SalesOrderItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseOrders",
                table: "PurchaseOrders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseOrderItems",
                table: "PurchaseOrderItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Menus",
                table: "Menus",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuPermissions",
                table: "MenuPermissions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InitialQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SupplierBatchId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Batches_items_ItemId1",
                        column: x => x.ItemId1,
                        principalTable: "items",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Capacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemperatureControl = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LocationId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BatchId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryTransactions_Batches_BatchId1",
                        column: x => x.BatchId1,
                        principalTable: "Batches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryTransactions_Locations_LocationId1",
                        column: x => x.LocationId1,
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryTransactions_items_ItemId1",
                        column: x => x.ItemId1,
                        principalTable: "items",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LocationId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Damaged = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reserved = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemLocations_Locations_LocationId1",
                        column: x => x.LocationId1,
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemLocations_items_ItemId1",
                        column: x => x.ItemId1,
                        principalTable: "items",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Batches_ItemId1",
                table: "Batches",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_BatchId1",
                table: "InventoryTransactions",
                column: "BatchId1");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_ItemId1",
                table: "InventoryTransactions",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_LocationId1",
                table: "InventoryTransactions",
                column: "LocationId1");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLocations_ItemId1",
                table: "ItemLocations",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLocations_LocationId1",
                table: "ItemLocations",
                column: "LocationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_items_Categories_CategoryId",
                table: "items",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_items_Vendors_VendorId",
                table: "items",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuPermissions_Menus_MenuId1",
                table: "MenuPermissions",
                column: "MenuId1",
                principalTable: "Menus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuPermissions_Permissions_PermissionId1",
                table: "MenuPermissions",
                column: "PermissionId1",
                principalTable: "Permissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Menus_ParentId",
                table: "Menus",
                column: "ParentId",
                principalTable: "Menus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_ModuleCategories_CategoryId",
                table: "Modules",
                column: "CategoryId",
                principalTable: "ModuleCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItems_Locations_LocationId1",
                table: "PurchaseOrderItems",
                column: "LocationId1",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId1",
                table: "PurchaseOrderItems",
                column: "PurchaseOrderId1",
                principalTable: "PurchaseOrders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItems_items_ItemId1",
                table: "PurchaseOrderItems",
                column: "ItemId1",
                principalTable: "items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Vendors_VendorId1",
                table: "PurchaseOrders",
                column: "VendorId1",
                principalTable: "Vendors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "RolePermissions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrderItems_Batches_BatchId1",
                table: "SalesOrderItems",
                column: "BatchId1",
                principalTable: "Batches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrderItems_SalesOrders_SalesOrderId1",
                table: "SalesOrderItems",
                column: "SalesOrderId1",
                principalTable: "SalesOrders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrderItems_items_ItemId1",
                table: "SalesOrderItems",
                column: "ItemId1",
                principalTable: "items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantModules_Modules_ModuleId",
                table: "TenantModules",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantModules_tenants_TenantId",
                table: "TenantModules",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
