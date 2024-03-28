using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class purchasenewfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpectedDate",
                schema: "Docs",
                table: "PurchaseOrders",
                type: "datetimeoffset(3)",
                precision: 3,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OperatorId",
                schema: "Docs",
                table: "PurchaseOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_OperatorId",
                schema: "Docs",
                table: "PurchaseOrders",
                column: "OperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Operators_OperatorId",
                schema: "Docs",
                table: "PurchaseOrders",
                column: "OperatorId",
                principalSchema: "Registry",
                principalTable: "Operators",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Operators_OperatorId",
                schema: "Docs",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_OperatorId",
                schema: "Docs",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ExpectedDate",
                schema: "Docs",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "OperatorId",
                schema: "Docs",
                table: "PurchaseOrders");
        }
    }
}
