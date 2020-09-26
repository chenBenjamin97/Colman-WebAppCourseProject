using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppProject.Migrations
{
    public partial class ContactApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactApplication",
                columns: table => new
                {
                    ContactAppID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    ContactType = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    ImgPath = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactApplication", x => x.ContactAppID);
                    table.ForeignKey(
                        name: "FK_ContactApplication_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactApplication_UserID",
                table: "ContactApplication",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactApplication");
        }
    }
}
