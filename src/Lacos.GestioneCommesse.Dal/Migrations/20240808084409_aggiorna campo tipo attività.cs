using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class aggiornacampotipoattività : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotUpdateJobStatus",
                schema: "Registry",
                table: "ActivityTypes",
                newName: "InfluenceJobStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InfluenceJobStatus",
                schema: "Registry",
                table: "ActivityTypes",
                newName: "NotUpdateJobStatus");
        }
    }
}
