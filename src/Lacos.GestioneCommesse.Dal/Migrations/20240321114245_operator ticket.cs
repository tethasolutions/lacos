using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class operatorticket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OperatorId",
                schema: "Docs",
                table: "Tickets",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_OperatorId",
                schema: "Docs",
                table: "Tickets",
                column: "OperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Operators_OperatorId",
                schema: "Docs",
                table: "Tickets",
                column: "OperatorId",
                principalSchema: "Registry",
                principalTable: "Operators",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Operators_OperatorId",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_OperatorId",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "OperatorId",
                schema: "Docs",
                table: "Tickets");
        }
    }
}
