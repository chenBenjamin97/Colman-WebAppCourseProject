using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppProject.Migrations
{
    public partial class BackToOriginal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaterMeterLastReadDateDate",
                table: "WaterTransactions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "WaterMeterLastReadDate",
                table: "WaterTransactions",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WaterMeterLastReadDate",
                table: "WaterTransactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<string>(
                name: "WaterMeterLastReadDateDate",
                table: "WaterTransactions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
