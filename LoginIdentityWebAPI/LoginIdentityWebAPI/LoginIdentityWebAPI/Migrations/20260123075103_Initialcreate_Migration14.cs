using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginIdentityWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initialcreate_Migration14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestRoleClaims_TestRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "TestRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestUserClaims_TestUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "TestUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_TestUserLogins_TestUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "TestUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_TestUserRoles_TestRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "TestRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestUserRoles_TestUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "TestUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_TestUserTokens_TestUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "TestUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserMainDetails",
                columns: table => new
                {
                    EmpId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmpPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMainDetails", x => x.EmpId);
                    table.ForeignKey(
                        name: "FK_UserMainDetails_TestUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "TestUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDetails",
                columns: table => new
                {
                    EmpId = table.Column<int>(type: "int", nullable: false),
                    EmpRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpDepartment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpPosition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpJoiningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmpStatusDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmpSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpState = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpDOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmpGender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpMaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpNationality = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.EmpId);
                    table.ForeignKey(
                        name: "FK_UserDetails_UserMainDetails_EmpId",
                        column: x => x.EmpId,
                        principalTable: "UserMainDetails",
                        principalColumn: "EmpId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestRoleClaims_RoleId",
                table: "TestRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "TestRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TestUserClaims_UserId",
                table: "TestUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestUserLogins_UserId",
                table: "TestUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestUserRoles_RoleId",
                table: "TestUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "TestUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "TestUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserMainDetails_ApplicationUserId",
                table: "UserMainDetails",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestRoleClaims");

            migrationBuilder.DropTable(
                name: "TestUserClaims");

            migrationBuilder.DropTable(
                name: "TestUserLogins");

            migrationBuilder.DropTable(
                name: "TestUserRoles");

            migrationBuilder.DropTable(
                name: "TestUserTokens");

            migrationBuilder.DropTable(
                name: "UserDetails");

            migrationBuilder.DropTable(
                name: "TestRoles");

            migrationBuilder.DropTable(
                name: "UserMainDetails");

            migrationBuilder.DropTable(
                name: "TestUsers");
        }
    }
}
