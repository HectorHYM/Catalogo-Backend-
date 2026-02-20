using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CatalogoBackend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCategoryIdFromUudiToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eliminando FK de tbl_products
            migrationBuilder.DropForeignKey(name: "tbl_products_category_fkey", table: "tbl_products");

            // Eliminando columna tipo UUID de tbl_products
            migrationBuilder.DropColumn(name: "category_id", table: "tbl_products");

            // Eliminando PK tipo UUID de tbl_categories
            migrationBuilder.DropPrimaryKey(name: "PK_tbl_categories", table: "tbl_categories");
            migrationBuilder.DropColumn(name: "id", table: "tbl_categories");

            // Agregando nueva PK tipo int a tbl_categories
            migrationBuilder.AddColumn<int>(name: "id", table: "tbl_categories", nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy",
                Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(name: "tbl_categories_pkey", table: "tbl_categories", column: "id");

            // Agregando nueva FK de tipo int en tbl_products
            migrationBuilder.AddColumn<int>(name: "category_id", table: "tbl_products", nullable: false);

            migrationBuilder.AddForeignKey(
                name: "tbl_products_category_fkey", 
                table: "tbl_products", 
                column: "category_id",
                principalTable: "tbl_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
