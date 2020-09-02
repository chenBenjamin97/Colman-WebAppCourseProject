using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppProject.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    EnteranceDate = table.Column<DateTime>(nullable: false),
                    PropertyCity = table.Column<string>(nullable: true),
                    PropertyStreet = table.Column<string>(nullable: true),
                    PropertyStreetNumber = table.Column<string>(nullable: true),
                    ApartmentNumber = table.Column<int>(nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "ElectricityTransactions",
                columns: table => new
                {
                    ElectricityMeterID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    ElectricityMeterLastRead = table.Column<float>(nullable: false),
                    ImgPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectricityTransactions", x => x.ElectricityMeterID);
                    table.ForeignKey(
                        name: "FK_ElectricityTransactions_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaterTransactions",
                columns: table => new
                {
                    WaterMeterID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    WaterMeterLastReadDate = table.Column<float>(nullable: false),
                    ImgPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterTransactions", x => x.WaterMeterID);
                    table.ForeignKey(
                        name: "FK_WaterTransactions_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElectricityTransactions_UserID",
                table: "ElectricityTransactions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_WaterTransactions_UserID",
                table: "WaterTransactions",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElectricityTransactions");

            migrationBuilder.DropTable(
                name: "WaterTransactions");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
