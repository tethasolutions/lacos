using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Link_Ticket_to_Job_instead_Intervention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Interventions_InterventionId",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "InterventionId",
                schema: "Docs",
                table: "Tickets",
                newName: "JobId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_InterventionId",
                schema: "Docs",
                table: "Tickets",
                newName: "IX_Tickets_JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Jobs_JobId",
                schema: "Docs",
                table: "Tickets",
                column: "JobId",
                principalSchema: "Docs",
                principalTable: "Jobs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Jobs_JobId",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "JobId",
                schema: "Docs",
                table: "Tickets",
                newName: "InterventionId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_JobId",
                schema: "Docs",
                table: "Tickets",
                newName: "IX_Tickets_InterventionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Interventions_InterventionId",
                schema: "Docs",
                table: "Tickets",
                column: "InterventionId",
                principalSchema: "Docs",
                principalTable: "Interventions",
                principalColumn: "Id");
        }
    }
}
