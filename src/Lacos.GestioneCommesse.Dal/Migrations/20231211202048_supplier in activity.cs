using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class supplierinactivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SupplierId",
                schema: "Docs",
                table: "Activities",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_SupplierId",
                schema: "Docs",
                table: "Activities",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Suppliers_SupplierId",
                schema: "Docs",
                table: "Activities",
                column: "SupplierId",
                principalSchema: "Registry",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Suppliers_SupplierId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_SupplierId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                schema: "Docs",
                table: "Activities");
        }
    }
}
