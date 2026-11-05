using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mandys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchStorageHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "branch_storage_histories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    branch_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    folio = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branch_storage_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_branch_storage_histories_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "branch_storage_history_products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    branch_storage_history_id = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branch_storage_history_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_branch_storage_history_products_branch_storage_histories_br",
                        column: x => x.branch_storage_history_id,
                        principalTable: "branch_storage_histories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_branch_storage_history_products_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "branch_id", "created_at", "deleted_at", "email", "father_last_name", "first_name", "mother_last_name", "password", "role", "updated_at" },
                values: new object[] { 3, null, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "operaciones@mandyspos.com", "Operaciones", "Gerente", null, "$argon2id$v=19$m=16,t=2,p=1$bWFuZHlzcG9z$C8kgZO6/V+MkFbWFE5pl9Q", "Gerente de Operaciones", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "ix_branch_storage_histories_branch_id",
                table: "branch_storage_histories",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_branch_storage_histories_branch_id_folio",
                table: "branch_storage_histories",
                columns: new[] { "branch_id", "folio" });

            migrationBuilder.CreateIndex(
                name: "ix_branch_storage_history_products_branch_storage_history_id",
                table: "branch_storage_history_products",
                column: "branch_storage_history_id");

            migrationBuilder.CreateIndex(
                name: "ix_branch_storage_history_products_product_id",
                table: "branch_storage_history_products",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "branch_storage_history_products");

            migrationBuilder.DropTable(
                name: "branch_storage_histories");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 3);
        }
    }
}
