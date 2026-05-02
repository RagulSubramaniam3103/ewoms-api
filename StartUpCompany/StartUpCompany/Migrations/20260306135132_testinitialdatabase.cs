using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StartUpCompany.Migrations
{
    /// <inheritdoc />
    public partial class testinitialdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentIdentityRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentIdentityRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentIdentityUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_StudentIdentityUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentIdentityRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentIdentityRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentIdentityRoleClaims_StudentIdentityRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "StudentIdentityRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterAdmin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdminName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCareerStart = table.Column<bool>(type: "bit", nullable: true),
                    PreviousSchool = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminJoiningDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdminEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Village = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pincode = table.Column<long>(type: "bigint", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterAdmin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterAdmin_StudentIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "StudentIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterStaff",
                columns: table => new
                {
                    StaffId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StaffUniqueCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCareerStart = table.Column<bool>(type: "bit", nullable: true),
                    StaffPreviousSchool = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Staffjoining = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StaffEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Village = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pincode = table.Column<long>(type: "bigint", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterStaff", x => x.StaffId);
                    table.ForeignKey(
                        name: "FK_MasterStaff_StudentIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "StudentIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterUsers",
                columns: table => new
                {
                    StudId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentUniqueCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFreshStudent = table.Column<bool>(type: "bit", nullable: true),
                    StudPreviousSchool = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentJoining = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StudDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Village = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pincode = table.Column<long>(type: "bigint", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterUsers", x => x.StudId);
                    table.ForeignKey(
                        name: "FK_MasterUsers_StudentIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "StudentIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentIdentityUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentIdentityUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentIdentityUserClaims_StudentIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "StudentIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentIdentityUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentIdentityUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_StudentIdentityUserLogins_StudentIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "StudentIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentIdentityUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentIdentityUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_StudentIdentityUserRoles_StudentIdentityRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "StudentIdentityRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentIdentityUserRoles_StudentIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "StudentIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentIdentityUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentIdentityUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_StudentIdentityUserTokens_StudentIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "StudentIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MasterAdmin_UserId",
                table: "MasterAdmin",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MasterStaff_UserId",
                table: "MasterStaff",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MasterUsers_UserId",
                table: "MasterUsers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentIdentityRoleClaims_RoleId",
                table: "StudentIdentityRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "StudentIdentityRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StudentIdentityUserClaims_UserId",
                table: "StudentIdentityUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentIdentityUserLogins_UserId",
                table: "StudentIdentityUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentIdentityUserRoles_RoleId",
                table: "StudentIdentityUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "StudentIdentityUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "StudentIdentityUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MasterAdmin");

            migrationBuilder.DropTable(
                name: "MasterStaff");

            migrationBuilder.DropTable(
                name: "MasterUsers");

            migrationBuilder.DropTable(
                name: "StudentIdentityRoleClaims");

            migrationBuilder.DropTable(
                name: "StudentIdentityUserClaims");

            migrationBuilder.DropTable(
                name: "StudentIdentityUserLogins");

            migrationBuilder.DropTable(
                name: "StudentIdentityUserRoles");

            migrationBuilder.DropTable(
                name: "StudentIdentityUserTokens");

            migrationBuilder.DropTable(
                name: "StudentIdentityRoles");

            migrationBuilder.DropTable(
                name: "StudentIdentityUsers");
        }
    }
}
