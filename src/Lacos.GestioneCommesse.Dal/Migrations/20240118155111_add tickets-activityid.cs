using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class addticketsactivityid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ActivityId",
                schema: "Docs",
                table: "Tickets",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ActivityId",
                schema: "Docs",
                table: "Tickets",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Activities_ActivityId",
                schema: "Docs",
                table: "Tickets",
                column: "ActivityId",
                principalSchema: "Docs",
                principalTable: "Activities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Activities_ActivityId",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ActivityId",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                schema: "Docs",
                table: "Tickets");
        }
    }
}
