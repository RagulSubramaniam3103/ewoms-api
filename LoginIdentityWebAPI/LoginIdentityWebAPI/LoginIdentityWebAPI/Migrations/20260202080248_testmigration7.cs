using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginIdentityWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class testmigration7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BookingStatus",
                table: "FlightTravelDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingStatus",
                table: "FlightTravelDetails");
        }
    }
}
