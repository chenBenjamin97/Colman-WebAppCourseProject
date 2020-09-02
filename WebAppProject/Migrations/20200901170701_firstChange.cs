using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppProject.Migrations
{
    public partial class firstChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WaterMeterLastReadDate",
                table: "WaterTransactions",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ElectricityMeterLastRead",
                table: "ElectricityTransactions",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "WaterMeterLastReadDate",
                table: "WaterTransactions",
                type: "real",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "ElectricityMeterLastRead",
                table: "ElectricityTransactions",
                type: "real",
                nullable: false,
                oldClrType: typeof(DateTime));
        }
    }
}
