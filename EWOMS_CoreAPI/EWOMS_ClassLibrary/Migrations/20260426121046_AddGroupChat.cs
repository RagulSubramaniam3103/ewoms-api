using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EWOMS_ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReceiverId",
                table: "EWO_ChatMessage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "EWO_ChatMessage",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EWO_ChatGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EWO_ChatGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EWO_ChatGroupMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EWO_ChatGroupMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EWO_ChatGroupMember_EWO_ChatGroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "EWO_ChatGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EWO_ChatGroupMember_EWO_MasterUser_UserId",
                        column: x => x.UserId,
                        principalTable: "EWO_MasterUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EWO_ChatGroupMember_GroupId",
                table: "EWO_ChatGroupMember",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_EWO_ChatGroupMember_UserId",
                table: "EWO_ChatGroupMember",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EWO_ChatGroupMember");

            migrationBuilder.DropTable(
                name: "EWO_ChatGroup");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "EWO_ChatMessage");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverId",
                table: "EWO_ChatMessage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
