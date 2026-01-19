using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class addinterventionfee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ExtraFee",
                schema: "Docs",
                table: "Interventions",
                type: "decimal(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceCallFee",
                schema: "Docs",
                table: "Interventions",
                type: "decimal(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceFee",
                schema: "Docs",
                table: "Interventions",
                type: "decimal(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TravelFee",
                schema: "Docs",
                table: "Interventions",
                type: "decimal(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraFee",
                schema: "Docs",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "ServiceCallFee",
                schema: "Docs",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "ServiceFee",
                schema: "Docs",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "TravelFee",
                schema: "Docs",
                table: "Interventions");
        }
    }
}
