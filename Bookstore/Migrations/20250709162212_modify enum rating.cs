using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class modifyenumrating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "65848ac4-1a28-401b-a6e2-64e8aba61c63");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cf200694-ca48-461d-a857-4c5b00e558ba");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "458ca70a-9ac2-4cad-95b7-be17c7482c54", null, "Customer", null },
                    { "4b801c92-1ac7-4fe0-b4bf-d52975b13fa3", null, "Admin", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "458ca70a-9ac2-4cad-95b7-be17c7482c54");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4b801c92-1ac7-4fe0-b4bf-d52975b13fa3");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "65848ac4-1a28-401b-a6e2-64e8aba61c63", null, "Admin", null },
                    { "cf200694-ca48-461d-a857-4c5b00e558ba", null, "Customer", null }
                });
        }
    }
}
