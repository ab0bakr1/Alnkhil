using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alnakhil.Migrations
{
    /// <inheritdoc />
    public partial class Addinventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Sales",
                newName: "SaleDate");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "SaleItems",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "TotalCost",
                table: "Purchases",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Purchases",
                newName: "PurchaseDate");

            migrationBuilder.RenameColumn(
                name: "PurchasePrice",
                table: "PurchaseItems",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Sales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupplierName",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "SupplierName",
                table: "Purchases");

            migrationBuilder.RenameColumn(
                name: "SaleDate",
                table: "Sales",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "SaleItems",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Purchases",
                newName: "TotalCost");

            migrationBuilder.RenameColumn(
                name: "PurchaseDate",
                table: "Purchases",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "PurchaseItems",
                newName: "PurchasePrice");
        }
    }
}
