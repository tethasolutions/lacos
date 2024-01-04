using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Source_From_Activity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_PurchaseOrders_SourcePuchaseOrderId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Tickets_SourceTicketId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_SourcePuchaseOrderId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_SourceTicketId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "SourcePuchaseOrderId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "SourceTicketId",
                schema: "Docs",
                table: "Activities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SourcePuchaseOrderId",
                schema: "Docs",
                table: "Activities",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SourceTicketId",
                schema: "Docs",
                table: "Activities",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_SourcePuchaseOrderId",
                schema: "Docs",
                table: "Activities",
                column: "SourcePuchaseOrderId",
                unique: true,
                filter: "[SourcePuchaseOrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_SourceTicketId",
                schema: "Docs",
                table: "Activities",
                column: "SourceTicketId",
                unique: true,
                filter: "[SourceTicketId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_PurchaseOrders_SourcePuchaseOrderId",
                schema: "Docs",
                table: "Activities",
                column: "SourcePuchaseOrderId",
                principalSchema: "Docs",
                principalTable: "PurchaseOrders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Tickets_SourceTicketId",
                schema: "Docs",
                table: "Activities",
                column: "SourceTicketId",
                principalSchema: "Docs",
                principalTable: "Tickets",
                principalColumn: "Id");
        }
    }
}
