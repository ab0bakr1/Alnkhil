using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alnakhil.Migrations
{
    /// <inheritdoc />
    public partial class AddProductsAndCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SellPrice",
                table: "Products",
                newName: "PriceBuy");

            migrationBuilder.RenameColumn(
                name: "BuyPrice",
                table: "Products",
                newName: "Price");

            migrationBuilder.AddColumn<int>(
                name: "MinQuantity",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinQuantity",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "PriceBuy",
                table: "Products",
                newName: "SellPrice");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "BuyPrice");
        }
    }
}
