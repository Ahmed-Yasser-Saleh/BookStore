using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class addattributesforbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "335b5ae8-9cbb-4ba1-a898-39ada32997a3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b2f1e500-9da2-4759-8f0e-f6f010a2019e");

            migrationBuilder.AddColumn<bool>(
                name: "Isfavourite",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "rate",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "65848ac4-1a28-401b-a6e2-64e8aba61c63", null, "Admin", null },
                    { "cf200694-ca48-461d-a857-4c5b00e558ba", null, "Customer", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "65848ac4-1a28-401b-a6e2-64e8aba61c63");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cf200694-ca48-461d-a857-4c5b00e558ba");

            migrationBuilder.DropColumn(
                name: "Isfavourite",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "rate",
                table: "Books");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "335b5ae8-9cbb-4ba1-a898-39ada32997a3", null, "Customer", null },
                    { "b2f1e500-9da2-4759-8f0e-f6f010a2019e", null, "Admin", null }
                });
        }
    }
}
