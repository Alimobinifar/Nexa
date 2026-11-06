using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LastMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropColumn(
                name: "PersianDate",
                table: "ProductEntries");

            migrationBuilder.RenameColumn(
                name: "ProductQuaantity",
                table: "SaleRegistrations",
                newName: "ProductQuantity");

            migrationBuilder.RenameColumn(
                name: "SaleChannelId",
                table: "ProductChannelAllocations",
                newName: "SoldQuantity");

            migrationBuilder.RenameColumn(
                name: "QuantitiesByChannel",
                table: "ProductChannelAllocations",
                newName: "ChannelId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SalesChannels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "SalesChannels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "FixedFee",
                table: "SalesChannels",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SupportPhone",
                table: "SalesChannels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "SalesChannels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CommissionRate",
                table: "SaleRegistrations",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "BuyPrice",
                table: "SaleRegistrations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionCost",
                table: "SaleRegistrations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NetProfit",
                table: "SaleRegistrations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NetProfitPercent",
                table: "SaleRegistrations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchaseNetCost",
                table: "SaleRegistrations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalSaleAmount",
                table: "SaleRegistrations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "EntryDate",
                table: "ProductEntries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "AllocatedQuantity",
                table: "ProductChannelAllocations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WarehouseInventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrentInventory = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseInventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseInventory_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseInventory_ProductId",
                table: "WarehouseInventory",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseInventory");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SalesChannels");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "SalesChannels");

            migrationBuilder.DropColumn(
                name: "FixedFee",
                table: "SalesChannels");

            migrationBuilder.DropColumn(
                name: "SupportPhone",
                table: "SalesChannels");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "SalesChannels");

            migrationBuilder.DropColumn(
                name: "BuyPrice",
                table: "SaleRegistrations");

            migrationBuilder.DropColumn(
                name: "CommissionCost",
                table: "SaleRegistrations");

            migrationBuilder.DropColumn(
                name: "NetProfit",
                table: "SaleRegistrations");

            migrationBuilder.DropColumn(
                name: "NetProfitPercent",
                table: "SaleRegistrations");

            migrationBuilder.DropColumn(
                name: "PurchaseNetCost",
                table: "SaleRegistrations");

            migrationBuilder.DropColumn(
                name: "TotalSaleAmount",
                table: "SaleRegistrations");

            migrationBuilder.DropColumn(
                name: "EntryDate",
                table: "ProductEntries");

            migrationBuilder.DropColumn(
                name: "AllocatedQuantity",
                table: "ProductChannelAllocations");

            migrationBuilder.RenameColumn(
                name: "ProductQuantity",
                table: "SaleRegistrations",
                newName: "ProductQuaantity");

            migrationBuilder.RenameColumn(
                name: "SoldQuantity",
                table: "ProductChannelAllocations",
                newName: "SaleChannelId");

            migrationBuilder.RenameColumn(
                name: "ChannelId",
                table: "ProductChannelAllocations",
                newName: "QuantitiesByChannel");

            migrationBuilder.AlterColumn<int>(
                name: "CommissionRate",
                table: "SaleRegistrations",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "PersianDate",
                table: "ProductEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentInventory = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_ProductId",
                table: "Inventories",
                column: "ProductId");
        }
    }
}
