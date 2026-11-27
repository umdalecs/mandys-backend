using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mandys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerLoyaltyPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "points",
                table: "users",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "customer_id",
                table: "orders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "points_earned",
                table: "orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                column: "points",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 2,
                column: "points",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 3,
                column: "points",
                value: 0m);

            migrationBuilder.CreateIndex(
                name: "ix_orders_customer_id",
                table: "orders",
                column: "customer_id");

            migrationBuilder.AddForeignKey(
                name: "fk_orders_users_customer_id",
                table: "orders",
                column: "customer_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_orders_users_customer_id",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "ix_orders_customer_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "points",
                table: "users");

            migrationBuilder.DropColumn(
                name: "customer_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "points_earned",
                table: "orders");
        }
    }
}
