using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingIdxOnCategoryIdTableProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_tbl_products_category_id",
                schema: "catalog",
                table: "tbl_products",
                column: "category_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_tbl_products_category_id",
                schema: "catalog",
                table: "tbl_products");
        }
    }
}
