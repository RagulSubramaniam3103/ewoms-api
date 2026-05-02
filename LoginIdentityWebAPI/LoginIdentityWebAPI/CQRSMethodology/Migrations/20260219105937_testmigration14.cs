using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CQRSMethodology.Migrations
{
    /// <inheritdoc />
    public partial class testmigration14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerDetails",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerDetails", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCartPurchased",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Customer_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCartPurchased", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCartPurchased_CustomerDetails_Customer_Id",
                        column: x => x.Customer_Id,
                        principalTable: "CustomerDetails",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCartPurchased_Customer_Id",
                table: "CustomerCartPurchased",
                column: "Customer_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerCartPurchased");

            migrationBuilder.DropTable(
                name: "CustomerDetails");
        }
    }
}
