using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TernakSepatu.Migrations
{
    /// <inheritdoc />
    public partial class InsertInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3590f978-fe47-4f23-af8d-ce9fe9e543ba", null, "admin", "admin" },
                    { "4cee0e02-6543-4df5-8c73-137b3f4e34b6", null, "costumer", "costumer" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3590f978-fe47-4f23-af8d-ce9fe9e543ba");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4cee0e02-6543-4df5-8c73-137b3f4e34b6");
        }
    }
}
