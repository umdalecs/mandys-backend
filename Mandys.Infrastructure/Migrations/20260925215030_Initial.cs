using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mandys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "branches",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    warehouse_only = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    is_supply = table.Column<bool>(type: "boolean", nullable: false),
                    price = table.Column<string>(type: "text", nullable: false),
                    measure_unit = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    branch_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    token_hash = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "id", "created_at", "deleted_at", "description", "is_deleted", "is_supply", "measure_unit", "price", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Jitomate saladet", false, true, "kg", "28.50", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lechuga romana", false, true, "pieza", "18.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cebolla blanca", false, true, "kg", "32.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Papa blanca para freír", false, true, "kg", "26.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Carne molida de res 80/20", false, true, "kg", "165.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pechuga de pollo sin hueso", false, true, "kg", "120.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pan para hamburguesa con ajonjolí", false, true, "pieza", "8.50", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Queso americano en rebanadas", false, true, "kg", "145.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tocino ahumado", false, true, "kg", "210.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mayonesa", false, true, "litro", "68.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Catsup", false, true, "litro", "45.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mostaza amarilla", false, true, "litro", "42.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salsa BBQ", false, true, "litro", "55.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aderezo ranch", false, true, "litro", "72.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aceite vegetal", false, true, "litro", "48.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sal refinada", false, true, "kg", "14.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Refresco de cola en lata", false, true, "pieza", "16.50", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Servilletas", false, true, "paquete", "38.00", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "branch_id", "created_at", "deleted_at", "email", "first_name", "is_deleted", "last_name", "password", "role", "updated_at" },
                values: new object[] { 1, null, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@mandys.com", "Administrator", false, "Administrator", "$argon2id$v=19$m=16,t=2,p=1$YWRtaW5pc3RyYXRvcnNhbHQ$n/2qmo8rW3KVIHy7g2Y0XA", "administrador", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token_hash",
                table: "refresh_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_branch_id",
                table: "users",
                column: "branch_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "branches");
        }
    }
}
