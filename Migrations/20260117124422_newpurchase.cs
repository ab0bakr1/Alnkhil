using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alnakhil.Migrations
{
    /// <inheritdoc />
    public partial class newpurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManualSalePrice",
                table: "PurchaseItems");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "PurchaseItems",
                newName: "PurchasePrice");

            migrationBuilder.AlterColumn<string>(
                name: "SupplierName",
                table: "Purchases",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNumber",
                table: "Purchases",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "Purchases",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentage",
                table: "Purchases",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "PurchaseItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "PurchaseItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_CategoryId",
                table: "PurchaseItems",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Categories_CategoryId",
                table: "PurchaseItems",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Categories_CategoryId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_CategoryId",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "PurchaseItems");

            migrationBuilder.RenameColumn(
                name: "PurchasePrice",
                table: "PurchaseItems",
                newName: "UnitPrice");

            migrationBuilder.AlterColumn<string>(
                name: "SupplierName",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNumber",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<decimal>(
                name: "ManualSalePrice",
                table: "PurchaseItems",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
