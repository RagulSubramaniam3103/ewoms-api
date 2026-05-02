using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginIdentityWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTimezonemigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "AirportDetails");

            migrationBuilder.CreateTable(
                name: "timeZoneairports",
                columns: table => new
                {
                    Tno = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AirpotCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeZoneName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UtcOffsetHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timeZoneairports", x => x.Tno);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "timeZoneairports");

            migrationBuilder.AddColumn<int>(
                name: "TimeZoneId",
                table: "AirportDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
