using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginIdentityWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class testmigration4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlightTravelDetails",
                columns: table => new
                {
                    FlightId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightTravelDetails", x => x.FlightId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightTravelDetails");
        }
    }
}
