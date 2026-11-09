using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSaleRegistrationRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleRegistrations_Products_ProductId",
                table: "SaleRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleRegistrations_SalesChannels_SalesChannelId",
                table: "SaleRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_SaleRegistrations_SalesChannelId",
                table: "SaleRegistrations");

            migrationBuilder.DropColumn(
                name: "SalesChannelId",
                table: "SaleRegistrations");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SalesChannels",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultCommissionRate",
                table: "SalesChannels",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CommissionRate",
                table: "SaleRegistrations",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_SaleRegistrations_ChannelId",
                table: "SaleRegistrations",
                column: "ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleRegistrations_Products_ProductId",
                table: "SaleRegistrations",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleRegistrations_SalesChannels_ChannelId",
                table: "SaleRegistrations",
                column: "ChannelId",
                principalTable: "SalesChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleRegistrations_Products_ProductId",
                table: "SaleRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleRegistrations_SalesChannels_ChannelId",
                table: "SaleRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_SaleRegistrations_ChannelId",
                table: "SaleRegistrations");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SalesChannels",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultCommissionRate",
                table: "SalesChannels",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "CommissionRate",
                table: "SaleRegistrations",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AddColumn<int>(
                name: "SalesChannelId",
                table: "SaleRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SaleRegistrations_SalesChannelId",
                table: "SaleRegistrations",
                column: "SalesChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleRegistrations_Products_ProductId",
                table: "SaleRegistrations",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleRegistrations_SalesChannels_SalesChannelId",
                table: "SaleRegistrations",
                column: "SalesChannelId",
                principalTable: "SalesChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
