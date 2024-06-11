using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroPortal.Migrations
{
    public partial class CascadeConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "26e46eb2-e468-426a-8351-43684a4612c6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f8a48ca-686d-4e7b-b948-ed1af631327b");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1f1f01dc-4c79-4dbb-b06d-cb732972b7b4", "1922c610-086b-4039-9f01-d03bdb9ff0ba", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f1f4dcbe-7b8a-4a65-ac1b-bf541913bc76", "753e5cc0-1b69-4672-bf8d-7cc75c26a530", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1f1f01dc-4c79-4dbb-b06d-cb732972b7b4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f1f4dcbe-7b8a-4a65-ac1b-bf541913bc76");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "26e46eb2-e468-426a-8351-43684a4612c6", "7e670c74-1e61-4d34-bf39-bd84bcfdb69a", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5f8a48ca-686d-4e7b-b948-ed1af631327b", "29887875-ceb0-4592-915c-c54a51ffb02a", "Admin", "ADMIN" });
        }
    }
}
