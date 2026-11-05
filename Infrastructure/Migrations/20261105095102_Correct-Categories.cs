using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrectCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CaegoryCode",
                table: "ProductCategories",
                newName: "CategoryCode");

            migrationBuilder.AddColumn<int>(
                name: "ParentCategoryId",
                table: "ProductCategories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "ProductCategories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ParentCategoryId",
                table: "ProductCategories",
                column: "ParentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_ProductCategories_ParentCategoryId",
                table: "ProductCategories",
                column: "ParentCategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_ProductCategories_ParentCategoryId",
                table: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_ParentCategoryId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "ParentCategoryId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ProductCategories");

            migrationBuilder.RenameColumn(
                name: "CategoryCode",
                table: "ProductCategories",
                newName: "CaegoryCode");
        }
    }
}
