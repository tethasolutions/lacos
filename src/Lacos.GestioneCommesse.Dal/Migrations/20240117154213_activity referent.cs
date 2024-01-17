using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class activityreferent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ReferentId",
                schema: "Docs",
                table: "Activities",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ReferentId",
                schema: "Docs",
                table: "Activities",
                column: "ReferentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Operators_ReferentId",
                schema: "Docs",
                table: "Activities",
                column: "ReferentId",
                principalSchema: "Registry",
                principalTable: "Operators",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Operators_ReferentId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_ReferentId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "ReferentId",
                schema: "Docs",
                table: "Activities");
        }
    }
}
