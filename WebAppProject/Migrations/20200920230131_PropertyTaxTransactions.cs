using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppProject.Migrations
{
    public partial class PropertyTaxTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyTax");

            migrationBuilder.CreateTable(
                name: "PropertyTaxTransactions",
                columns: table => new
                {
                    PropertyID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    ImgPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyTaxTransactions", x => x.PropertyID);
                    table.ForeignKey(
                        name: "FK_PropertyTaxTransactions_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ViewModel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewModel", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTaxTransactions_UserID",
                table: "PropertyTaxTransactions",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyTaxTransactions");

            migrationBuilder.DropTable(
                name: "ViewModel");

            migrationBuilder.CreateTable(
                name: "PropertyTax",
                columns: table => new
                {
                    PropertyID = table.Column<int>(type: "int", nullable: false),
                    ImgPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: false)
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
    }
}
