using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class aggiuntacampotipoattivitàinordiniacquisto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ActivityTypeId",
                schema: "Docs",
                table: "PurchaseOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_ActivityTypeId",
                schema: "Docs",
                table: "PurchaseOrders",
                column: "ActivityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_ActivityTypes_ActivityTypeId",
                schema: "Docs",
                table: "PurchaseOrders",
                column: "ActivityTypeId",
                principalSchema: "Registry",
                principalTable: "ActivityTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_ActivityTypes_ActivityTypeId",
                schema: "Docs",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_ActivityTypeId",
                schema: "Docs",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ActivityTypeId",
                schema: "Docs",
                table: "PurchaseOrders");
        }
    }
}
