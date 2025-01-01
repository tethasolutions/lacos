using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class aggiuntacampopurchaseorderidinticket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PurchaseOrderId",
                schema: "Docs",
                table: "Tickets",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PurchaseOrderId",
                schema: "Docs",
                table: "Tickets",
                column: "PurchaseOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_PurchaseOrders_PurchaseOrderId",
                schema: "Docs",
                table: "Tickets",
                column: "PurchaseOrderId",
                principalSchema: "Docs",
                principalTable: "PurchaseOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_PurchaseOrders_PurchaseOrderId",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_PurchaseOrderId",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId",
                schema: "Docs",
                table: "Tickets");
        }
    }
}
