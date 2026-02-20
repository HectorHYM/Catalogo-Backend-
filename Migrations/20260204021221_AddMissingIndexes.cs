using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(name: "idx_tbl_users_username", schema: "catalog", table: "tbl_users", column: "username", unique: true);
            migrationBuilder.CreateIndex(name: "idx_tbl_users_email", schema: "catalog", table: "tbl_users", column: "email", unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "idx_tbl_users_username", schema: "catalog", table: "tbl_users");
            migrationBuilder.DropIndex(name: "idx_tbl_users_email", schema: "catalog", table: "tbl_users");
        }
    }
}
