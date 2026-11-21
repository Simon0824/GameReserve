using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservations.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExcludeFailedReservationsFromOverlapConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""CREATE EXTENSION IF NOT EXISTS btree_gist SCHEMA public;""");
            
            migrationBuilder.Sql("""
                ALTER TABLE "Reservations"
                ADD CONSTRAINT "EX_Reservations_Dates_NoOverlapWhenNotFailed"
                EXCLUDE USING gist (
                    "GameId" WITH =,
                    tstzrange("StartDate", "EndDate", '[)') WITH &&
                )
                WHERE ("Status" <> 'Failed');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            ALTER Table "Reservations"
            DROP CONSTRAINT "EX_Reservations_Dates_NoOverlapWhenNotFailed"
            """);
        }
    }
}
