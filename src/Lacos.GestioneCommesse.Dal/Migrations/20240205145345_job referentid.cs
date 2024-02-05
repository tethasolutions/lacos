using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class jobreferentid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ReferentId",
                schema: "Docs",
                table: "Jobs",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ReferentId",
                schema: "Docs",
                table: "Jobs",
                column: "ReferentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Operators_ReferentId",
                schema: "Docs",
                table: "Jobs",
                column: "ReferentId",
                principalSchema: "Registry",
                principalTable: "Operators",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Operators_ReferentId",
                schema: "Docs",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_ReferentId",
                schema: "Docs",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ReferentId",
                schema: "Docs",
                table: "Jobs");
        }
    }
}
