using Microsoft.EntityFrameworkCore.Migrations;

namespace MyUniversity.UserManager.Repository.Migrations
{
    public partial class UniversityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Universities",
                columns: table => new
                {
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.TenantId);
                });

            migrationBuilder.InsertData(
                table: "Universities",
                columns: new[] { "TenantId", "Address", "EmailAddress", "Name", "PhoneNumber" },
                values: new object[] { "100ba3c7-2e71-43e1-9c20-8b5f51bb930e", "University Street, 1, Lviv, Lviv region, 79000", "IvanFrankoUniversity@gmail.com", "Ivan Franko National University of Lviv", "+380 (50) 123 4567" });

            migrationBuilder.CreateIndex(
                name: "IX_Universities_Address",
                table: "Universities",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Universities_EmailAddress",
                table: "Universities",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Universities_Name",
                table: "Universities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Universities_PhoneNumber",
                table: "Universities",
                column: "PhoneNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Universities");
        }
    }
}
