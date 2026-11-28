using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mandys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDrawerReconciliations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "drawer_reconciliations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    branch_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    performed_by_user_id = table.Column<int>(type: "integer", nullable: false),
                    expected_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    counted_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    discrepancy = table.Column<decimal>(type: "numeric", nullable: false),
                    has_discrepancy = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    resolved_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    resolved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    adjustment_transaction_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drawer_reconciliations", x => x.id);
                    table.ForeignKey(
                        name: "fk_drawer_reconciliations_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_drawer_reconciliations_drawer_transactions_adjustment_trans",
                        column: x => x.adjustment_transaction_id,
                        principalTable: "drawer_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_drawer_reconciliations_users_performed_by_user_id",
                        column: x => x.performed_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_drawer_reconciliations_users_resolved_by_user_id",
                        column: x => x.resolved_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_drawer_reconciliations_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_drawer_reconciliations_adjustment_transaction_id",
                table: "drawer_reconciliations",
                column: "adjustment_transaction_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_drawer_reconciliations_branch_id",
                table: "drawer_reconciliations",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_drawer_reconciliations_branch_id_user_id",
                table: "drawer_reconciliations",
                columns: new[] { "branch_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_drawer_reconciliations_created_at",
                table: "drawer_reconciliations",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_drawer_reconciliations_performed_by_user_id",
                table: "drawer_reconciliations",
                column: "performed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_drawer_reconciliations_resolved_by_user_id",
                table: "drawer_reconciliations",
                column: "resolved_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_drawer_reconciliations_status",
                table: "drawer_reconciliations",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_drawer_reconciliations_user_id",
                table: "drawer_reconciliations",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "drawer_reconciliations");
        }
    }
}
