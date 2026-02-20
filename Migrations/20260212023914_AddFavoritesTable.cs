using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoritesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_favorites",
                schema: "catalog",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tbl_favorites", x => new { x.user_id, x.product_id });
                    table.ForeignKey(
                        name: "tbl_favorites_product_id_fkey",
                        column: x => x.product_id,
                        principalSchema: "catalog",
                        principalTable: "tbl_products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "tbl_favorites_user_id_fkey",
                        column: x => x.user_id,
                        principalSchema: "catalog",
                        principalTable: "tbl_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_tbl_favorites_product_id",
                schema: "catalog",
                table: "tbl_favorites",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "idx_tbl_favorites_user_id",
                schema: "catalog",
                table: "tbl_favorites",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_favorites",
                schema: "catalog");
        }
    }
}
