using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class addaddressinticket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AddressId",
                schema: "Docs",
                table: "Tickets",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AddressId",
                schema: "Docs",
                table: "Tickets",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Addresses_AddressId",
                schema: "Docs",
                table: "Tickets",
                column: "AddressId",
                principalSchema: "Registry",
                principalTable: "Addresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Addresses_AddressId",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_AddressId",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AddressId",
                schema: "Docs",
                table: "Tickets");
        }
    }
}
