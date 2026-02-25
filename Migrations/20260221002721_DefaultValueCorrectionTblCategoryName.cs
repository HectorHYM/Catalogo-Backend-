using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoBackend.Migrations
{
    /// <inheritdoc />
    public partial class DefaultValueCorrectionTblCategoryName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "catalog",
                table: "tbl_categories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldDefaultValue: "others");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "catalog",
                table: "tbl_categories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "others",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
