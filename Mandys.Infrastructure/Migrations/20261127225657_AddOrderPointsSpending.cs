using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mandys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderPointsSpending : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "points_spent",
                table: "orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "subtotal_amount",
                table: "orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "points_spent",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "subtotal_amount",
                table: "orders");
        }
    }
}
