using Microsoft.EntityFrameworkCore.Migrations;

namespace MyUniversity.UserManager.Repository.Migrations
{
    public partial class AddDefaultServiceUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Universities_TenantId",
                table: "Users");

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 3, 1 },
                    { 4, 1 }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt", "TenantId" },
                values: new object[] { new byte[] { 127, 26, 201, 117, 70, 201, 48, 108, 51, 207, 47, 13, 130, 101, 243, 22, 230, 233, 25, 97, 207, 95, 90, 129, 15, 76, 192, 69, 41, 88, 112, 170, 238, 246, 216, 163, 26, 131, 197, 211, 90, 156, 87, 160, 97, 222, 127, 217, 153, 245, 225, 245, 232, 202, 53, 42, 60, 144, 157, 10, 116, 101, 37, 15 }, new byte[] { 103, 23, 71, 255, 237, 126, 38, 253, 48, 82, 222, 133, 194, 58, 131, 140, 23, 137, 187, 241, 170, 203, 66, 143, 229, 153, 158, 62, 93, 194, 173, 52, 171, 196, 52, 91, 61, 93, 12, 237, 78, 161, 247, 59, 161, 243, 222, 140, 150, 176, 68, 160, 18, 235, 80, 37, 145, 116, 67, 25, 73, 26, 127, 3, 67, 57, 9, 16, 175, 163, 182, 209, 5, 15, 153, 250, 28, 9, 132, 195, 146, 230, 202, 240, 133, 99, 68, 98, 193, 33, 136, 57, 148, 160, 157, 58, 72, 27, 72, 131, 36, 114, 108, 201, 47, 234, 75, 245, 81, 19, 107, 242, 157, 61, 244, 199, 190, 48, 194, 126, 121, 205, 99, 255, 119, 233, 208, 58 }, null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "EmailAddress", "FirstName", "LastName", "PasswordHash", "PasswordSalt", "PhoneNumber", "TenantId" },
                values: new object[] { 2, "UserManager.Service@gmail.com", "UserManager", "Service", new byte[] { 127, 26, 201, 117, 70, 201, 48, 108, 51, 207, 47, 13, 130, 101, 243, 22, 230, 233, 25, 97, 207, 95, 90, 129, 15, 76, 192, 69, 41, 88, 112, 170, 238, 246, 216, 163, 26, 131, 197, 211, 90, 156, 87, 160, 97, 222, 127, 217, 153, 245, 225, 245, 232, 202, 53, 42, 60, 144, 157, 10, 116, 101, 37, 15 }, new byte[] { 103, 23, 71, 255, 237, 126, 38, 253, 48, 82, 222, 133, 194, 58, 131, 140, 23, 137, 187, 241, 170, 203, 66, 143, 229, 153, 158, 62, 93, 194, 173, 52, 171, 196, 52, 91, 61, 93, 12, 237, 78, 161, 247, 59, 161, 243, 222, 140, 150, 176, 68, 160, 18, 235, 80, 37, 145, 116, 67, 25, 73, 26, 127, 3, 67, 57, 9, 16, 175, 163, 182, 209, 5, 15, 153, 250, 28, 9, 132, 195, 146, 230, 202, 240, 133, 99, 68, 98, 193, 33, 136, 57, 148, 160, 157, 58, 72, 27, 72, 131, 36, 114, 108, 201, 47, 234, 75, 245, 81, 19, 107, 242, 157, 61, 244, 199, 190, 48, 194, 126, 121, 205, 99, 255, 119, 233, 208, 58 }, "000", null });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 2, 2 });

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Universities_TenantId",
                table: "Users",
                column: "TenantId",
                principalTable: "Universities",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Universities_TenantId",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt", "TenantId" },
                values: new object[] { new byte[] { 74, 76, 212, 16, 193, 168, 161, 83, 174, 210, 87, 83, 181, 138, 203, 61, 60, 198, 194, 196, 239, 11, 215, 248, 57, 47, 43, 138, 32, 122, 159, 230, 114, 74, 197, 146, 208, 24, 156, 208, 226, 140, 10, 169, 169, 125, 255, 11, 113, 139, 105, 72, 162, 81, 32, 114, 181, 49, 4, 71, 75, 50, 210, 151 }, new byte[] { 51, 231, 238, 53, 127, 178, 100, 131, 224, 127, 74, 9, 162, 182, 60, 124, 3, 69, 8, 246, 162, 173, 158, 116, 92, 46, 253, 253, 223, 7, 101, 254, 67, 0, 137, 221, 134, 90, 214, 60, 32, 134, 174, 174, 34, 38, 168, 187, 209, 28, 175, 222, 235, 54, 125, 61, 190, 60, 233, 103, 209, 61, 73, 44, 238, 59, 219, 209, 165, 155, 145, 151, 219, 46, 123, 210, 54, 113, 42, 11, 186, 149, 162, 10, 176, 243, 150, 22, 172, 129, 200, 9, 56, 160, 116, 254, 147, 148, 231, 24, 115, 40, 224, 38, 202, 33, 145, 219, 198, 174, 250, 79, 37, 213, 224, 55, 186, 184, 198, 28, 26, 182, 25, 121, 200, 254, 67, 242 }, "11116258-9207-4bdc-b101-fb560cc8cb20" });

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Universities_TenantId",
                table: "Users",
                column: "TenantId",
                principalTable: "Universities",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
