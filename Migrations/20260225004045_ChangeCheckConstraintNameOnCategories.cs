using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoBackend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCheckConstraintNameOnCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "chk_name",
                schema: "catalog",
                table: "tbl_categories");

            migrationBuilder.AddCheckConstraint(
                name: "chk_name",
                schema: "catalog",
                table: "tbl_categories",
                sql: "name IN ('electronics', 'home', 'clothes', 'sports', 'beauty', 'games', 'toys', 'healthy', 'automotive', 'books', 'yard', 'tools', 'pets', 'children', 'jewelry', 'others')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "chk_name",
                schema: "catalog",
                table: "tbl_categories");

            migrationBuilder.AddCheckConstraint(
                name: "chk_name",
                schema: "catalog",
                table: "tbl_categories",
                sql: "name IN ('all', 'electronics', 'home', 'clothes', 'sports', 'beauty', 'games', 'toys', 'healthy', 'automotive', 'books', 'yard', 'tools', 'pets', 'children', 'jewelry', 'others')");
        }
    }
}
