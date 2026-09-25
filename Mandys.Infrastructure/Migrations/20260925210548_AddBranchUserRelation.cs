using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mandys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "branch_id",
                table: "users",
                type: "integer",
                nullable: true);

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

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                column: "branch_id",
                value: null);

            migrationBuilder.CreateIndex(
                name: "ix_users_branch_id",
                table: "users",
                column: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_branches_branch_id",
                table: "users",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_branches_branch_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "branches");

            migrationBuilder.DropIndex(
                name: "ix_users_branch_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "branch_id",
                table: "users");
        }
    }
}
