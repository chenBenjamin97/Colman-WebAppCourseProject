using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppProject.Migrations
{
    public partial class PropertyTax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropertyTax",
                columns: table => new
                {
                    PropertyID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    ImgPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyTax", x => x.PropertyID);
                    table.ForeignKey(
                        name: "FK_PropertyTax_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTax_UserID",
                table: "PropertyTax",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyTax");
        }
    }
}
