using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alnakhil.Migrations
{
    /// <inheritdoc />
    public partial class reAddCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "PurchaseItems",
                type: "int",
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
                principalColumn: "Id");
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
                name: "CategoryId",
                table: "PurchaseItems");
        }
    }
}
