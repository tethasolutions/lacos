using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class addexpensesrelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderExpenses_PurchaseOrders_PurchaseOrderId",
                schema: "Docs",
                table: "PurchaseOrderExpenses");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderExpenses_PurchaseOrders_PurchaseOrderId",
                schema: "Docs",
                table: "PurchaseOrderExpenses",
                column: "PurchaseOrderId",
                principalSchema: "Docs",
                principalTable: "PurchaseOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderExpenses_PurchaseOrders_PurchaseOrderId",
                schema: "Docs",
                table: "PurchaseOrderExpenses");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderExpenses_PurchaseOrders_PurchaseOrderId",
                schema: "Docs",
                table: "PurchaseOrderExpenses",
                column: "PurchaseOrderId",
                principalSchema: "Docs",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
