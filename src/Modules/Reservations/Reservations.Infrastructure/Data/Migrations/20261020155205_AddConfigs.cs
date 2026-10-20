using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservations.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InboxMessages",
                table: "InboxMessages");

            migrationBuilder.EnsureSchema(
                name: "reservations");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                newName: "outbox_messages",
                newSchema: "reservations");

            migrationBuilder.RenameTable(
                name: "InboxMessages",
                newName: "inbox_messages",
                newSchema: "reservations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_outbox_messages",
                schema: "reservations",
                table: "outbox_messages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_inbox_messages",
                schema: "reservations",
                table: "inbox_messages",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_outbox_messages",
                schema: "reservations",
                table: "outbox_messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_inbox_messages",
                schema: "reservations",
                table: "inbox_messages");

            migrationBuilder.RenameTable(
                name: "outbox_messages",
                schema: "reservations",
                newName: "OutboxMessages");

            migrationBuilder.RenameTable(
                name: "inbox_messages",
                schema: "reservations",
                newName: "InboxMessages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InboxMessages",
                table: "InboxMessages",
                column: "Id");
        }
    }
}
