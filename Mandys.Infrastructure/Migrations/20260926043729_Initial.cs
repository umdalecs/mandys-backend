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
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    warehouse_only = table.Column<bool>(type: "boolean", nullable: false),
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
                name: "combos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_combos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    points = table.Column<decimal>(type: "numeric", nullable: false),
                    email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dishes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dishes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_supply = table.Column<bool>(type: "boolean", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    measure_unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                name: "combo_dishes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    combo_id = table.Column<int>(type: "integer", nullable: false),
                    dish_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_combo_dishes", x => x.id);
                    table.ForeignKey(
                        name: "fk_combo_dishes_combos_combo_id",
                        column: x => x.combo_id,
                        principalTable: "combos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_combo_dishes_dishes_dish_id",
                        column: x => x.dish_id,
                        principalTable: "dishes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "combo_products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    combo_id = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_combo_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_combo_products_combos_combo_id",
                        column: x => x.combo_id,
                        principalTable: "combos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_combo_products_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dish_products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dish_id = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dish_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_dish_products_dishes_dish_id",
                        column: x => x.dish_id,
                        principalTable: "dishes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dish_products_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                table: "combos",
                columns: new[] { "id", "created_at", "deleted_at", "is_deleted", "name", "price", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Combo Hamburguesa clásica", 129m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Combo Hamburguesa doble", 169m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Combo Hamburguesa de pollo", 139m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Combo Tacos al pastor", 119m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Combo Tacos de bistec", 125m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Combo Burrito", 145m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Combo Quesadillas", 125m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Combo Ensalada", 119m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Combo Postre", 119m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Combo Familiar", 349m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "dishes",
                columns: new[] { "id", "created_at", "deleted_at", "is_deleted", "name", "price", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Hamburguesa clásica", 85m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Hamburguesa doble", 125m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Hamburguesa de pollo", 92m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Taco al pastor", 48m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Taco de bistec", 52m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Burrito de carne", 98m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Quesadilla de pollo", 78m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Ensalada César", 95m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Churros con cajeta", 65m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Sopa de tortilla", 55m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "id", "created_at", "deleted_at", "description", "is_deleted", "is_supply", "measure_unit", "price", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Jitomate saladet", false, true, "kg", 28.50m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Jitomate cherry", false, true, "kg", 55.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lechuga romana", false, true, "pieza", 18.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lechuga iceberg", false, true, "pieza", 22.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cebolla blanca", false, true, "kg", 32.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cebolla morada", false, true, "kg", 38.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cebolla cambray", false, true, "manojo", 25.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Papa blanca para freír", false, true, "kg", 26.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pepino", false, true, "kg", 24.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Chile jalapeño", false, true, "kg", 35.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aguacate hass", false, true, "kg", 85.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Limón sin semilla", false, true, "kg", 30.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cilantro fresco", false, true, "manojo", 12.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Zanahoria", false, true, "kg", 22.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Champiñón blanco", false, true, "kg", 95.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pimiento morrón verde", false, true, "kg", 48.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pimiento morrón rojo", false, true, "kg", 62.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Apio", false, true, "kg", 28.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ajo fresco", false, true, "kg", 110.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Elote amarillo", false, true, "pieza", 9.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Calabacita", false, true, "kg", 26.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Espinaca baby", false, true, "kg", 120.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Col blanca", false, true, "kg", 20.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Rábano", false, true, "kg", 24.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Nopal limpio", false, true, "kg", 30.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Piña miel", false, true, "kg", 25.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mango ataulfo", false, true, "kg", 40.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 28, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Fresa", false, true, "kg", 70.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 29, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Plátano tabasco", false, true, "kg", 22.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 30, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Manzana roja", false, true, "kg", 45.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 31, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Naranja para jugo", false, true, "kg", 20.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 32, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Perejil fresco", false, true, "manojo", 12.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 33, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Epazote fresco", false, true, "manojo", 10.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 34, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Chayote", false, true, "kg", 18.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 35, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Betabel", false, true, "kg", 26.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 36, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Carne molida de res 80/20", false, true, "kg", 165.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 37, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Carne molida de res 90/10", false, true, "kg", 185.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 38, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Arrachera marinada", false, true, "kg", 240.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 39, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Milanesa de res", false, true, "kg", 190.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 40, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pechuga de pollo sin hueso", false, true, "kg", 120.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 41, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Muslo de pollo sin hueso", false, true, "kg", 95.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 42, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tiras de pollo", false, true, "kg", 115.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 43, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Milanesa de pollo", false, true, "kg", 125.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 44, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tocino ahumado", false, true, "kg", 210.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 45, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Jamón de pavo", false, true, "kg", 130.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 46, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Chuleta ahumada", false, true, "kg", 150.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 47, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salchicha para asar", false, true, "kg", 90.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 48, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Longaniza", false, true, "kg", 110.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 49, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pepperoni", false, true, "kg", 220.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 50, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Huevo fresco", false, true, "kg", 52.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 51, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Huevo líquido pasteurizado", false, true, "litro", 65.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 52, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Filete de pescado empanizado", false, true, "kg", 140.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 53, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Camarón mediano sin cáscara", false, true, "kg", 260.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 54, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Atún enlatado", false, true, "lata", 32.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 55, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Costilla de cerdo", false, true, "kg", 145.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 56, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pulled pork", false, true, "kg", 175.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 57, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salami", false, true, "kg", 200.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 58, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Chorizo argentino", false, true, "kg", 135.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 59, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pavo molido", false, true, "kg", 150.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 60, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lomo de cerdo", false, true, "kg", 130.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 61, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pan para hamburguesa con ajonjolí", false, true, "pieza", 8.50m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 62, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pan brioche para hamburguesa", false, true, "pieza", 11.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 63, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pan integral para hamburguesa", false, true, "pieza", 10.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 64, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pan de papa para hamburguesa", false, true, "pieza", 12.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 65, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pan para hot dog", false, true, "pieza", 7.50m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 66, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pan telera", false, true, "pieza", 6.50m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 67, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pan pita", false, true, "pieza", 9.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 68, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tortilla de harina", false, true, "pieza", 3.50m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 69, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tortilla de maíz", false, true, "pieza", 2.50m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 70, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Totopos de maíz", false, true, "kg", 55.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 71, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pan molido para empanizar", false, true, "kg", 48.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 72, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Crotones sazonados", false, true, "kg", 85.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 73, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Queso americano en rebanadas", false, true, "kg", 145.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 74, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Queso cheddar rallado", false, true, "kg", 160.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 75, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Queso mozzarella rallado", false, true, "kg", 155.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 76, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Queso gouda en rebanadas", false, true, "kg", 175.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 77, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Queso asadero", false, true, "kg", 150.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 78, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Queso cotija rallado", false, true, "kg", 140.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 79, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Queso crema", false, true, "kg", 95.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 80, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mantequilla sin sal", false, true, "kg", 180.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 81, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Crema ácida", false, true, "litro", 60.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 82, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Leche entera", false, true, "litro", 28.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 83, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Leche deslactosada", false, true, "litro", 30.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 84, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Yogur natural", false, true, "litro", 45.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 85, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mayonesa", false, true, "litro", 68.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 86, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mayonesa chipotle", false, true, "litro", 78.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 87, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Catsup", false, true, "litro", 45.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 88, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mostaza amarilla", false, true, "litro", 42.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 89, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mostaza dijon", false, true, "litro", 95.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 90, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salsa BBQ", false, true, "litro", 55.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 91, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salsa BBQ picante", false, true, "litro", 62.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 92, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aderezo ranch", false, true, "litro", 72.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 93, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aderezo césar", false, true, "litro", 75.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 94, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aderezo mil islas", false, true, "litro", 70.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 95, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salsa de chipotle", false, true, "litro", 65.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 96, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salsa inglesa", false, true, "litro", 58.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 97, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salsa de soya", false, true, "litro", 48.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 98, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salsa verde envasada", false, true, "litro", 52.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 99, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salsa roja de árbol envasada", false, true, "litro", 56.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 100, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salsa de habanero envasada", false, true, "litro", 68.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 101, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salsa teriyaki", false, true, "litro", 82.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 102, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salsa agridulce", false, true, "litro", 60.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 103, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Alioli de ajo", false, true, "litro", 80.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 104, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salsa de tamarindo", false, true, "litro", 54.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 105, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sal refinada", false, true, "kg", 14.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 106, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sal de mar", false, true, "kg", 22.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 107, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pimienta negra molida", false, true, "kg", 180.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 108, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Paprika", false, true, "kg", 160.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 109, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ajo en polvo", false, true, "kg", 140.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 110, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cebolla en polvo", false, true, "kg", 130.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 111, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Orégano seco", false, true, "kg", 120.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 112, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Comino molido", false, true, "kg", 150.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 113, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Hoja de laurel", false, true, "kg", 200.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 114, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tomillo seco", false, true, "kg", 220.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 115, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Romero seco", false, true, "kg", 230.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 116, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Albahaca seca", false, true, "kg", 210.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 117, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Perejil seco", false, true, "kg", 110.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 118, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Chile de árbol seco", false, true, "kg", 170.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 119, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Chile chipotle seco", false, true, "kg", 190.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 120, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Consomé de pollo en polvo", false, true, "kg", 85.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 121, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sazonador tipo tajín", false, true, "kg", 95.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 122, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Azúcar estándar", false, true, "kg", 32.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 123, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Azúcar mascabado", false, true, "kg", 45.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 124, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Miel de abeja", false, true, "litro", 150.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 125, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aceite vegetal", false, true, "litro", 48.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 126, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aceite de oliva extra virgen", false, true, "litro", 180.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 127, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aceite en aerosol", false, true, "pieza", 95.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 128, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Vinagre blanco", false, true, "litro", 25.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 129, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Vinagre de manzana", false, true, "litro", 42.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 130, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Vinagre balsámico", false, true, "litro", 120.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 131, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Jugo de limón embotellado", false, true, "litro", 38.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 132, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Manteca vegetal", false, true, "kg", 55.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 133, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Refresco de cola en lata", false, true, "pieza", 16.50m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 134, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Refresco de naranja en lata", false, true, "pieza", 16.50m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 135, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Refresco de limón en lata", false, true, "pieza", 16.50m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 136, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Refresco de manzana en lata", false, true, "pieza", 16.50m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 137, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agua embotellada 600ml", false, true, "pieza", 9.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 138, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agua mineral 600ml", false, true, "pieza", 12.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 139, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Jugo de naranja envasado", false, true, "litro", 35.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 140, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Café molido americano", false, true, "kg", 280.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 141, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Café descafeinado molido", false, true, "kg", 300.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 142, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Té negro en bolsitas", false, true, "pieza", 1.80m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 143, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Té verde en bolsitas", false, true, "pieza", 2.20m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 144, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Chocolate en polvo", false, true, "kg", 120.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 145, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Jarabe de vainilla", false, true, "litro", 110.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 146, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Jarabe de caramelo", false, true, "litro", 115.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 147, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Crema para café en polvo", false, true, "kg", 90.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 148, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mermelada de fresa", false, true, "kg", 75.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 149, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Papas corte recto congeladas", false, true, "kg", 65.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 150, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Papas gajo congeladas", false, true, "kg", 70.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 151, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aros de cebolla congelados", false, true, "kg", 85.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 152, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Nuggets de pollo congelados", false, true, "kg", 110.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 153, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Palitos de queso mozzarella congelados", false, true, "kg", 150.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 154, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Medallón de res congelado", false, true, "pieza", 38.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 155, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Filete de pollo empanizado congelado", false, true, "pieza", 32.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 156, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Elote amarillo congelado", false, true, "kg", 45.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 157, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mezcla de verduras congeladas", false, true, "kg", 50.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 158, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pulpa de mango congelada", false, true, "kg", 60.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 159, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Fresa congelada", false, true, "kg", 75.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 160, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Helado de vainilla", false, true, "litro", 85.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 161, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Hielo en bolsa 5kg", false, true, "bolsa", 35.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 162, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Masa de pizza congelada", false, true, "pieza", 28.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 163, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Harina de trigo", false, true, "kg", 28.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 164, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Fécula de maíz", false, true, "kg", 45.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 165, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Arroz blanco", false, true, "kg", 30.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 166, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Frijoles refritos en lata", false, true, "lata", 22.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 167, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Elote enlatado", false, true, "lata", 25.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 168, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Champiñones enlatados", false, true, "lata", 35.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 169, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Chiles jalapeños enlatados", false, true, "lata", 28.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 170, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aceitunas negras enlatadas", false, true, "lata", 55.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 171, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pepinillos en rodajas", false, true, "litro", 48.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 172, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Puré de tomate envasado", false, true, "litro", 32.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 173, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cacahuate tostado", false, true, "kg", 90.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 174, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ajonjolí", false, true, "kg", 110.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 175, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cajeta quemada", false, true, "litro", 95.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 176, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Chamoy envasado", false, true, "litro", 50.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 177, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Vasos de cartón 12oz", false, true, "pieza", 1.80m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 178, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Vasos de cartón 16oz", false, true, "pieza", 2.20m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 179, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tapas para vaso", false, true, "pieza", 0.90m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 180, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Popotes de papel", false, true, "pieza", 0.40m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 181, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Charolas de cartón", false, true, "pieza", 3.50m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 182, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cajas para hamburguesa", false, true, "pieza", 2.80m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 183, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Envolturas de papel encerado", false, true, "pieza", 0.60m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 184, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Bolsas de papel para llevar", false, true, "pieza", 1.50m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 185, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Servilletas", false, true, "paquete", 38.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 186, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Toallas de papel en rollo", false, true, "pieza", 45.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 187, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Guantes desechables", false, true, "caja", 120.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 188, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Papel aluminio en rollo", false, true, "pieza", 85.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 189, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Película plástica adherente", false, true, "pieza", 75.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 190, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Jabón líquido para manos", false, true, "litro", 55.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 191, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Queso parmesano rallado", false, true, "kg", 180.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 192, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salsa sriracha", false, true, "litro", 70.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 193, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aderezo de mostaza y miel", false, true, "litro", 76.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 194, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Jugo de manzana envasado", false, true, "litro", 34.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 195, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agua tónica en lata", false, true, "pieza", 15.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 196, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Té de manzanilla en bolsitas", false, true, "pieza", 1.80m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 197, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Champiñón portobello", false, true, "kg", 130.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 198, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cebolla perla", false, true, "kg", 42.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 199, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Manteca de cerdo", false, true, "kg", 60.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 200, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Queso oaxaca", false, true, "kg", 145.00m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 201, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Refresco de cola 600 ml", false, false, "pieza", 35m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 202, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Refresco de naranja 600 ml", false, false, "pieza", 35m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 203, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Limonada de limón natural 500 ml", false, false, "pieza", 38m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 204, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Té de manzanilla 500 ml", false, false, "pieza", 32m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 205, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Jugo de naranja natural 450 ml", false, false, "pieza", 48m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 206, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Batido de fresa 450 ml", false, false, "pieza", 52m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 207, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Malteada de vainilla 500 ml", false, false, "pieza", 78m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 208, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Espresso", false, false, "pieza", 28m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 209, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Capuchino", false, false, "pieza", 45m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 210, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Porción de papas fritas", false, false, "pieza", 45m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 211, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Nachos con queso", false, false, "pieza", 69m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 212, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Alitas de pollo Buffalo", false, false, "pieza", 89m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 213, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Bowl de pollo César", false, false, "pieza", 118m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 214, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Flan de caramelo", false, false, "pieza", 42m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 215, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pastel de chocolate", false, false, "pieza", 48m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "branch_id", "created_at", "deleted_at", "email", "first_name", "is_deleted", "last_name", "password", "role", "updated_at" },
                values: new object[] { 1, null, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@mandyspos.com", "Administrator", false, "Administrator", "$argon2id$v=19$m=16,t=2,p=1$YWRtaW5pc3RyYXRvcnNhbHQ$n/2qmo8rW3KVIHy7g2Y0XA", "administrador", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "combo_dishes",
                columns: new[] { "id", "combo_id", "created_at", "deleted_at", "dish_id", "is_deleted", "quantity", "updated_at" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, false, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 2, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, false, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 3, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, false, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 4, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, false, 2m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 5, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, false, 2m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 6, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 6, false, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 7, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 7, false, 2m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 8, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 8, false, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 9, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 9, false, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 10, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, false, 2m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 10, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, false, 2m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "combo_products",
                columns: new[] { "id", "combo_id", "created_at", "deleted_at", "is_deleted", "product_id", "quantity", "updated_at" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 201, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 1, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 210, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 2, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 202, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 2, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 210, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 3, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 203, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 3, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 210, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 4, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 201, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 5, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 202, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 6, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 204, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 6, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 211, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 7, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 205, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 8, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 203, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 9, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 209, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, 9, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 215, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, 10, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 201, 4m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, 10, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 210, 2m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "dish_products",
                columns: new[] { "id", "created_at", "deleted_at", "dish_id", "is_deleted", "product_id", "quantity", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, false, 61, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, false, 36, 0.15m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, false, 73, 0.04m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, false, 4, 2m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, false, 1, 0.05m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, false, 5, 0.03m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, false, 87, 0.02m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, false, 88, 0.01m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, false, 171, 0.02m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, false, 62, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, false, 36, 0.3m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, false, 75, 0.06m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, false, 73, 0.04m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, false, 1, 0.08m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, false, 5, 0.04m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, false, 44, 0.05m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, false, 90, 0.03m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, false, 64, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, false, 43, 0.18m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, false, 78, 0.03m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, false, 3, 2m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, false, 9, 0.04m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, false, 86, 0.02m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, false, 69, 2m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, false, 38, 0.15m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, false, 10, 0.02m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, false, 13, 0.05m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 28, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, false, 2, 0.05m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 29, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, false, 6, 0.03m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 30, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, false, 19, 0.01m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 31, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, false, 69, 2m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 32, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, false, 39, 0.12m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 33, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, false, 7, 0.08m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 34, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, false, 32, 0.03m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 35, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, false, 105, 0.002m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 36, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, false, 107, 0.001m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 37, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 6, false, 68, 1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 38, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 6, false, 36, 0.18m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 39, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 6, false, 80, 0.01m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 40, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 6, false, 172, 0.08m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 41, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 6, false, 22, 0.03m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 42, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 6, false, 14, 0.03m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 43, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 6, false, 82, 0.05m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 44, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 6, false, 77, 0.04m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 45, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 6, false, 91, 0.02m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 46, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 7, false, 68, 2m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 47, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 7, false, 42, 0.15m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 48, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 7, false, 73, 0.08m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 49, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 7, false, 17, 0.04m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 50, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 7, false, 92, 0.03m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 51, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 7, false, 125, 0.01m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 52, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 8, false, 22, 0.08m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 53, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 8, false, 23, 0.06m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 54, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 8, false, 74, 0.04m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 55, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 8, false, 93, 0.05m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 56, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 8, false, 171, 0.01m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 57, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 8, false, 71, 0.03m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 58, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 8, false, 105, 0.002m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 59, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 8, false, 107, 0.001m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 60, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 9, false, 163, 0.1m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 61, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 9, false, 132, 0.05m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 62, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 9, false, 105, 0.002m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 63, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 9, false, 175, 0.08m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 64, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 10, false, 1, 0.2m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 65, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 10, false, 69, 2m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 66, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 10, false, 5, 0.06m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 67, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 10, false, 19, 0.01m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 68, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 10, false, 125, 0.02m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 69, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 10, false, 80, 0.01m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 70, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, 10, false, 32, 0.02m, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "ix_combo_dishes_combo_id",
                table: "combo_dishes",
                column: "combo_id");

            migrationBuilder.CreateIndex(
                name: "ix_combo_dishes_dish_id",
                table: "combo_dishes",
                column: "dish_id");

            migrationBuilder.CreateIndex(
                name: "ix_combo_products_combo_id",
                table: "combo_products",
                column: "combo_id");

            migrationBuilder.CreateIndex(
                name: "ix_combo_products_product_id",
                table: "combo_products",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_dish_products_dish_id",
                table: "dish_products",
                column: "dish_id");

            migrationBuilder.CreateIndex(
                name: "ix_dish_products_product_id",
                table: "dish_products",
                column: "product_id");

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
                name: "combo_dishes");

            migrationBuilder.DropTable(
                name: "combo_products");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "dish_products");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "combos");

            migrationBuilder.DropTable(
                name: "dishes");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "branches");
        }
    }
}
