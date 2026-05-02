using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EWOMS_ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddChatAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Document",
                table: "EWO_ChatMessage",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "EWO_ChatMessage",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video",
                table: "EWO_ChatMessage",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Document",
                table: "EWO_ChatMessage");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "EWO_ChatMessage");

            migrationBuilder.DropColumn(
                name: "Video",
                table: "EWO_ChatMessage");
        }
    }
}
