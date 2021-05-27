using Microsoft.EntityFrameworkCore.Migrations;

namespace MyUniversity.UserManager.Repository.Migrations
{
    public partial class AddUserPasswordAndSaltHashes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Users",
                type: "varbinary(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Users",
                type: "varbinary(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 74, 76, 212, 16, 193, 168, 161, 83, 174, 210, 87, 83, 181, 138, 203, 61, 60, 198, 194, 196, 239, 11, 215, 248, 57, 47, 43, 138, 32, 122, 159, 230, 114, 74, 197, 146, 208, 24, 156, 208, 226, 140, 10, 169, 169, 125, 255, 11, 113, 139, 105, 72, 162, 81, 32, 114, 181, 49, 4, 71, 75, 50, 210, 151 }, new byte[] { 51, 231, 238, 53, 127, 178, 100, 131, 224, 127, 74, 9, 162, 182, 60, 124, 3, 69, 8, 246, 162, 173, 158, 116, 92, 46, 253, 253, 223, 7, 101, 254, 67, 0, 137, 221, 134, 90, 214, 60, 32, 134, 174, 174, 34, 38, 168, 187, 209, 28, 175, 222, 235, 54, 125, 61, 190, 60, 233, 103, 209, 61, 73, 44, 238, 59, 219, 209, 165, 155, 145, 151, 219, 46, 123, 210, 54, 113, 42, 11, 186, 149, 162, 10, 176, 243, 150, 22, 172, 129, 200, 9, 56, 160, 116, 254, 147, 148, 231, 24, 115, 40, 224, 38, 202, 33, 145, 219, 198, 174, 250, 79, 37, 213, 224, 55, 186, 184, 198, 28, 26, 182, 25, 121, 200, 254, 67, 242 } });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Users");
        }
    }
}
