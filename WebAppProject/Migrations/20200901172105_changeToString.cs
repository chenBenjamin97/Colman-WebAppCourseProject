using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppProject.Migrations
{
    public partial class changeToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WaterMeterLastReadDateDate",
                table: "WaterTransactions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaterMeterLastReadDateDate",
                table: "WaterTransactions");
        }
    }
}
