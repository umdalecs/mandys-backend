using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mandys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierNameAndReasonToBranchStorageHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "reason",
                table: "branch_storage_histories",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "supplier_name",
                table: "branch_storage_histories",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reason",
                table: "branch_storage_histories");

            migrationBuilder.DropColumn(
                name: "supplier_name",
                table: "branch_storage_histories");
        }
    }
}
