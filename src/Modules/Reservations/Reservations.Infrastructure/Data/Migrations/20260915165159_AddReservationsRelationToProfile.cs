using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservations.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationsRelationToProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_ReservationProfiles_ReservationProfileId",
                table: "Reservation");

            migrationBuilder.RenameColumn(
                name: "ReservationProfileId",
                table: "Reservation",
                newName: "ReservaiotnProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_ReservationProfileId",
                table: "Reservation",
                newName: "IX_Reservation_ReservaiotnProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_ReservationProfiles_ReservaiotnProfileId",
                table: "Reservation",
                column: "ReservaiotnProfileId",
                principalTable: "ReservationProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_ReservationProfiles_ReservaiotnProfileId",
                table: "Reservation");

            migrationBuilder.RenameColumn(
                name: "ReservaiotnProfileId",
                table: "Reservation",
                newName: "ReservationProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_ReservaiotnProfileId",
                table: "Reservation",
                newName: "IX_Reservation_ReservationProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_ReservationProfiles_ReservationProfileId",
                table: "Reservation",
                column: "ReservationProfileId",
                principalTable: "ReservationProfiles",
                principalColumn: "Id");
        }
    }
}
