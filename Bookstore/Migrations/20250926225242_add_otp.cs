using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class add_otp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "458ca70a-9ac2-4cad-95b7-be17c7482c54");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4b801c92-1ac7-4fe0-b4bf-d52975b13fa3");

            migrationBuilder.AddColumn<string>(
                name: "Admin_OTP",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OTP",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "621a136a-8faa-4823-be96-d0678c0cfd39", null, "Customer", null },
                    { "f07c3f5b-2071-4549-a7fe-96ab3f6f70e0", null, "Admin", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "621a136a-8faa-4823-be96-d0678c0cfd39");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f07c3f5b-2071-4549-a7fe-96ab3f6f70e0");

            migrationBuilder.DropColumn(
                name: "Admin_OTP",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OTP",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "458ca70a-9ac2-4cad-95b7-be17c7482c54", null, "Customer", null },
                    { "4b801c92-1ac7-4fe0-b4bf-d52975b13fa3", null, "Admin", null }
                });
        }
    }
}
