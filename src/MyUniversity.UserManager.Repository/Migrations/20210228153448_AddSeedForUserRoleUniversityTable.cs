using Microsoft.EntityFrameworkCore.Migrations;

namespace MyUniversity.UserManager.Repository.Migrations
{
    public partial class AddSeedForUserRoleUniversityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Universities",
                keyColumn: "TenantId",
                keyValue: "100ba3c7-2e71-43e1-9c20-8b5f51bb930e");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Role" },
                values: new object[,]
                {
                    { 1, "SuperAdmin" },
                    { 2, "Service" },
                    { 3, "UniversityAdmin" },
                    { 4, "Teacher" },
                    { 5, "Student" }
                });

            migrationBuilder.InsertData(
                table: "Universities",
                columns: new[] { "TenantId", "Address", "EmailAddress", "Name", "PhoneNumber" },
                values: new object[] { "11116258-9207-4bdc-b101-fb560cc8cb20", "University Street, 1, Lviv, Lviv region, 79000", "Ivan.Franko.University@lnu.edu.ua", "Ivan Franko National University of Lviv", "+380 (50) 123 4567" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "EmailAddress", "FirstName", "LastName", "PhoneNumber", "TenantId" },
                values: new object[] { 1, "Super.Admin@gmail.com", "Super", "Admin", "380 50 123 4567", "11116258-9207-4bdc-b101-fb560cc8cb20" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Universities",
                keyColumn: "TenantId",
                keyValue: "11116258-9207-4bdc-b101-fb560cc8cb20");

            migrationBuilder.InsertData(
                table: "Universities",
                columns: new[] { "TenantId", "Address", "EmailAddress", "Name", "PhoneNumber" },
                values: new object[] { "100ba3c7-2e71-43e1-9c20-8b5f51bb930e", "University Street, 1, Lviv, Lviv region, 79000", "IvanFrankoUniversity@gmail.com", "Ivan Franko National University of Lviv", "+380 (50) 123 4567" });
        }
    }
}
