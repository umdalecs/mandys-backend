using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mandys.Migrations
{
    /// <inheritdoc />
    public partial class Initialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false, defaultValue: "User"),
                    refresh_token = table.Column<string>(type: "text", nullable: true),
                    refresh_token_expiry_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "created_at", "deleted_at", "email", "first_name", "is_deleted", "last_name", "password", "refresh_token", "refresh_token_expiry_time", "role", "updated_at" },
                values: new object[] { new Guid("d1a7b9c3-4e56-4f89-a123-b456c789d012"), new DateTime(2026, 9, 9, 7, 0, 0, 0, DateTimeKind.Utc), null, "admin@mandys.com", "Administrator", false, "Administrator", "$argon2id$v=19$m=16,t=2,p=1$YWRtaW5pc3RyYXRvcnNhbHQ$n/2qmo8rW3KVIHy7g2Y0XA", null, null, "Admin", new DateTime(2026, 9, 9, 7, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
