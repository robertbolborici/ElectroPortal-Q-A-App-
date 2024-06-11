using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroPortal.Migrations
{
    public partial class CascadeConfigV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                values: new object[] { "7fe5e943-46be-4cc4-8d48-656247120d47", "7f6f8724-8d6f-4bba-8575-dbf78622aef7", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "fa82b450-e832-4ea9-9759-b671e711e900", "2b747352-78b4-4992-b123-6553d4f8eaf3", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7fe5e943-46be-4cc4-8d48-656247120d47");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fa82b450-e832-4ea9-9759-b671e711e900");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1f1f01dc-4c79-4dbb-b06d-cb732972b7b4", "1922c610-086b-4039-9f01-d03bdb9ff0ba", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f1f4dcbe-7b8a-4a65-ac1b-bf541913bc76", "753e5cc0-1b69-4672-bf8d-7cc75c26a530", "Admin", "ADMIN" });
        }
    }
}
