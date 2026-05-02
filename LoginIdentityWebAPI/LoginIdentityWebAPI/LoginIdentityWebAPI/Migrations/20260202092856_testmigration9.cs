using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginIdentityWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class testmigration9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "flightseatDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Businessclass = table.Column<bool>(type: "bit", nullable: false),
                    TotalSeat_Businessclass = table.Column<int>(type: "int", nullable: false),
                    Economicclass = table.Column<bool>(type: "bit", nullable: false),
                    TotalSeat_Economicclass = table.Column<int>(type: "int", nullable: false),
                    Firstclass = table.Column<bool>(type: "bit", nullable: false),
                    TotalSeat_Firstclass = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_flightseatDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlightSeatPrice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeatClass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightSeatPrice", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "flightseatDetails");

            migrationBuilder.DropTable(
                name: "FlightSeatPrice");
        }
    }
}
