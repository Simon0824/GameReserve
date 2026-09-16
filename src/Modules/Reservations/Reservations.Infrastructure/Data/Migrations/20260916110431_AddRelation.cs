using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservations.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_ReservationProfiles_ReservaiotnProfileId",
                table: "Reservation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservation",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_ReservaiotnProfileId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "ReservaiotnProfileId",
                table: "Reservation");

            migrationBuilder.RenameTable(
                name: "Reservation",
                newName: "Reservations");

            migrationBuilder.AddColumn<Guid>(
                name: "ReservationProfileId",
                table: "Reservations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReservationProfileId",
                table: "Reservations",
                column: "ReservationProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_ReservationProfiles_ReservationProfileId",
                table: "Reservations",
                column: "ReservationProfileId",
                principalTable: "ReservationProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_ReservationProfiles_ReservationProfileId",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ReservationProfileId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReservationProfileId",
                table: "Reservations");

            migrationBuilder.RenameTable(
                name: "Reservations",
                newName: "Reservation");

            migrationBuilder.AddColumn<Guid>(
                name: "ReservaiotnProfileId",
                table: "Reservation",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservation",
                table: "Reservation",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_ReservaiotnProfileId",
                table: "Reservation",
                column: "ReservaiotnProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_ReservationProfiles_ReservaiotnProfileId",
                table: "Reservation",
                column: "ReservaiotnProfileId",
                principalTable: "ReservationProfiles",
                principalColumn: "Id");
        }
    }
}
