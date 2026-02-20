using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesAndCategoryIdtoProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "category_id",
                schema: "catalog",
                table: "tbl_products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "tbl_categories",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "others")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_categories", x => x.id);
                    table.CheckConstraint("chk_name", "name IN ('all', 'electronics', 'home', 'clothes', 'sports', 'beauty', 'games', 'toys', 'healthy', 'automotive', 'books', 'yard', 'tools', 'pets', 'children', 'jewelry', 'others')");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_products_category_id",
                schema: "catalog",
                table: "tbl_products",
                column: "category_id");

            migrationBuilder.AddForeignKey(
                name: "tbl_products_category_fkey",
                schema: "catalog",
                table: "tbl_products",
                column: "category_id",
                principalSchema: "catalog",
                principalTable: "tbl_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "tbl_products_category_fkey",
                schema: "catalog",
                table: "tbl_products");

            migrationBuilder.DropTable(
                name: "tbl_categories",
                schema: "catalog");

            migrationBuilder.DropIndex(
                name: "IX_tbl_products_category_id",
                schema: "catalog",
                table: "tbl_products");

            migrationBuilder.DropColumn(
                name: "category_id",
                schema: "catalog",
                table: "tbl_products");
        }
    }
}
