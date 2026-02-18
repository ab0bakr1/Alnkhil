using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alnakhil.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "InventoryTransactions",
                newName: "QuantityChanged");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "InventoryTransactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "InventoryTransactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "InventoryTransactions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuantityAfter",
                table: "InventoryTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuantityBefore",
                table: "InventoryTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "QuantityAfter",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "QuantityBefore",
                table: "InventoryTransactions");

            migrationBuilder.RenameColumn(
                name: "QuantityChanged",
                table: "InventoryTransactions",
                newName: "Quantity");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "InventoryTransactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "InventoryTransactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
