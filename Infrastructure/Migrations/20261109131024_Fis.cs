using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductChannelAllocations_Products_ProductId",
                table: "ProductChannelAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductChannelAllocations_SalesChannels_SalesChannelId",
                table: "ProductChannelAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ProductChannelAllocations_ProductId",
                table: "ProductChannelAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ProductChannelAllocations_SalesChannelId",
                table: "ProductChannelAllocations");

            migrationBuilder.DropColumn(
                name: "SalesChannelId",
                table: "ProductChannelAllocations");

            migrationBuilder.CreateIndex(
                name: "IX_ProductChannelAllocations_ChannelId",
                table: "ProductChannelAllocations",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductChannelAllocations_ProductId_ChannelId",
                table: "ProductChannelAllocations",
                columns: new[] { "ProductId", "ChannelId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductChannelAllocations_Products_ProductId",
                table: "ProductChannelAllocations",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductChannelAllocations_SalesChannels_ChannelId",
                table: "ProductChannelAllocations",
                column: "ChannelId",
                principalTable: "SalesChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductChannelAllocations_Products_ProductId",
                table: "ProductChannelAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductChannelAllocations_SalesChannels_ChannelId",
                table: "ProductChannelAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ProductChannelAllocations_ChannelId",
                table: "ProductChannelAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ProductChannelAllocations_ProductId_ChannelId",
                table: "ProductChannelAllocations");

            migrationBuilder.AddColumn<int>(
                name: "SalesChannelId",
                table: "ProductChannelAllocations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductChannelAllocations_ProductId",
                table: "ProductChannelAllocations",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductChannelAllocations_SalesChannelId",
                table: "ProductChannelAllocations",
                column: "SalesChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductChannelAllocations_Products_ProductId",
                table: "ProductChannelAllocations",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductChannelAllocations_SalesChannels_SalesChannelId",
                table: "ProductChannelAllocations",
                column: "SalesChannelId",
                principalTable: "SalesChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
