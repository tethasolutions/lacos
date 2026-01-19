using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class addactivitytypenewbools : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasServiceFees",
                schema: "Registry",
                table: "ActivityTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdministrative",
                schema: "Registry",
                table: "ActivityTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasServiceFees",
                schema: "Registry",
                table: "ActivityTypes");

            migrationBuilder.DropColumn(
                name: "IsAdministrative",
                schema: "Registry",
                table: "ActivityTypes");
        }
    }
}
