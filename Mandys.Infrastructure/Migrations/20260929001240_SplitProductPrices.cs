using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mandys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitProductPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "price",
                table: "products",
                newName: "sale_price");

            migrationBuilder.AddColumn<decimal>(
                name: "cost_price",
                table: "products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 1,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 2,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 3,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 4,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 5,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 6,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 7,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 8,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 9,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 10,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 11,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 12,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 13,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 14,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 15,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 16,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 17,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 18,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 19,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 20,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 21,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 22,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 23,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 24,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 25,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 26,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 27,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 28,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 29,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 30,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 31,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 32,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 33,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 34,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 35,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 36,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 37,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 38,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 39,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 40,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 41,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 42,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 43,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 44,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 45,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 46,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 47,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 48,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 49,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 50,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 51,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 52,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 53,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 54,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 55,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 56,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 57,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 58,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 59,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 60,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 61,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 62,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 63,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 64,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 65,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 66,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 67,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 68,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 69,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 70,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 71,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 72,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 73,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 74,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 75,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 76,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 77,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 78,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 79,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 80,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 81,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 82,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 83,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 84,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 85,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 86,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 87,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 88,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 89,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 90,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 91,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 92,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 93,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 94,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 95,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 96,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 97,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 98,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 99,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 100,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 101,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 102,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 103,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 104,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 105,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 106,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 107,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 108,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 109,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 110,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 111,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 112,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 113,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 114,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 115,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 116,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 117,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 118,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 119,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 120,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 121,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 122,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 123,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 124,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 125,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 126,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 127,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 128,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 129,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 130,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 131,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 132,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 133,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 134,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 135,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 136,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 137,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 138,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 139,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 140,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 141,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 142,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 143,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 144,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 145,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 146,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 147,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 148,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 149,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 150,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 151,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 152,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 153,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 154,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 155,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 156,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 157,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 158,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 159,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 160,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 161,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 162,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 163,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 164,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 165,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 166,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 167,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 168,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 169,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 170,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 171,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 172,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 173,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 174,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 175,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 176,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 177,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 178,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 179,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 180,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 181,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 182,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 183,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 184,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 185,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 186,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 187,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 188,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 189,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 190,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 191,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 192,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 193,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 194,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 195,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 196,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 197,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 198,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 199,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 200,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 201,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 202,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 203,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 204,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 205,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 206,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 207,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 208,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 209,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 210,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 211,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 212,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 213,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 214,
                column: "cost_price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 215,
                column: "cost_price",
                value: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cost_price",
                table: "products");

            migrationBuilder.RenameColumn(
                name: "sale_price",
                table: "products",
                newName: "price");
        }
    }
}
