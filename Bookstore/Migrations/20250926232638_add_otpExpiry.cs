using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class add_otpExpiry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "621a136a-8faa-4823-be96-d0678c0cfd39");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f07c3f5b-2071-4549-a7fe-96ab3f6f70e0");

            migrationBuilder.AddColumn<DateTime>(
                name: "Admin_OTPExpiry",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OTPExpiry",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "be75126c-24a2-4b99-8ed0-06096ec79c3f", null, "Customer", null },
                    { "ff0de934-1fb5-4dba-bcdc-65ce191808ea", null, "Admin", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "be75126c-24a2-4b99-8ed0-06096ec79c3f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ff0de934-1fb5-4dba-bcdc-65ce191808ea");

            migrationBuilder.DropColumn(
                name: "Admin_OTPExpiry",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OTPExpiry",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "621a136a-8faa-4823-be96-d0678c0cfd39", null, "Customer", null },
                    { "f07c3f5b-2071-4549-a7fe-96ab3f6f70e0", null, "Admin", null }
                });
        }
    }
}
