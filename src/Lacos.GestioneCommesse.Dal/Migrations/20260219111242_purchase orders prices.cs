using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class purchaseordersprices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                schema: "Docs",
                table: "PurchaseOrderExpenses",
                newName: "TotalAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                schema: "Docs",
                table: "PurchaseOrderExpenses",
                type: "decimal(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                schema: "Docs",
                table: "PurchaseOrderItems",
                type: "decimal(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                schema: "Docs",
                table: "PurchaseOrderItems",
                type: "decimal(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                schema: "Docs",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                schema: "Docs",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                schema: "Docs",
                table: "PurchaseOrderExpenses");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                schema: "Docs",
                table: "PurchaseOrderExpenses",
                newName: "Amount");
        }
    }
}
