using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class updateactivitiesquotationreschedulenotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RescheduleNotes",
                schema: "Docs",
                table: "Interventions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasQuotation",
                schema: "Registry",
                table: "ActivityTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "QuotationAmount",
                schema: "Docs",
                table: "Activities",
                type: "decimal(14,2)",
                precision: 14,
                scale: 2,
                nullable: true);

            migrationBuilder.Sql("UPDATE Registry.ActivityTypes SET HasQuotation = 0; UPDATE Registry.ActivityTypes SET HasDependencies = 0 WHERE HasDependencies IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RescheduleNotes",
                schema: "Docs",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "HasQuotation",
                schema: "Registry",
                table: "ActivityTypes");

            migrationBuilder.DropColumn(
                name: "QuotationAmount",
                schema: "Docs",
                table: "Activities");
        }
    }
}
