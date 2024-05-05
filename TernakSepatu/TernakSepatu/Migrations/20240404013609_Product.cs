using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TernakSepatu.Migrations
{
    /// <inheritdoc />
    public partial class Product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });


            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandName = table.Column<string>(nullable: true),
                    ImageURL = table.Column<string>(nullable: true) // Tambahkan kolom ImageURL
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.Id);
                });


            migrationBuilder.CreateTable(
name: "SubCategory",
columns: table => new
{
Id = table.Column<int>(nullable: false)
.Annotation("SqlServer:Identity", "1, 1"),
SubCategoryName = table.Column<string>(nullable: true),
CategoryID = table.Column<int>(nullable: false),
},
constraints: table =>
{
table.PrimaryKey("PK_SubCategory", x => x.Id);
table.ForeignKey(
name: "FK_SubCategory_Category_CategoryID",
column: x => x.CategoryID,
principalTable: "Category",
principalColumn: "Id",
onDelete: ReferentialAction.Cascade);
});




            migrationBuilder.CreateTable(
    name: "Product",
    columns: table => new
    {
        Id = table.Column<int>(nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
        ProductName = table.Column<string>(nullable: true),
        CategoryID = table.Column<int>(nullable: false),
        SubCategoryID = table.Column<int>(nullable: false),
        BrandID = table.Column<int>(nullable: false),
        Size = table.Column<int>(nullable: false),
        Colors = table.Column<string>(nullable: true),
        Details = table.Column<string>(nullable: true),
        ImageURL = table.Column<string>(nullable: true),
        Price = table.Column<decimal>(nullable: false),
        Discount = table.Column<decimal>(nullable: true),
        Gender = table.Column<string>(nullable: true),
        Stock = table.Column<string>(nullable: true),
        Condition = table.Column<string>(nullable: true)

    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Product", x => x.Id);
        table.ForeignKey(
            name: "FK_Product_Brand_BrandID",
            column: x => x.BrandID,
            principalTable: "Brand",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict); // Mengubah onDelete menjadi ReferentialAction.Restrict
        table.ForeignKey(
            name: "FK_Product_Category_CategoryID",
            column: x => x.CategoryID,
            principalTable: "Category",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict); // Mengubah onDelete menjadi ReferentialAction.Restrict
        table.ForeignKey(
            name: "FK_Product_SubCategory_SubCategoryID",
            column: x => x.SubCategoryID,
            principalTable: "SubCategory",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict); // Mengubah onDelete menjadi ReferentialAction.Restrict
    });



            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(nullable: false),
                    UserID = table.Column<string>(nullable: true),
                    Rating = table.Column<decimal>(nullable: false),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Product_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_BrandID",
                table: "Product",
                column: "BrandID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryID",
                table: "Product",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_SubCategoryID",
                table: "Product",
                column: "SubCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Review_ProductID",
                table: "Review",
                column: "ProductID");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropTable(
                name: "SubCategory");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
