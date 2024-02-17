using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class nuovicampiattività : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusLabel0",
                schema: "Registry",
                table: "ActivityTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusLabel1",
                schema: "Registry",
                table: "ActivityTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusLabel2",
                schema: "Registry",
                table: "ActivityTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Informations",
                schema: "Docs",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                schema: "Docs",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusLabel0",
                schema: "Registry",
                table: "ActivityTypes");

            migrationBuilder.DropColumn(
                name: "StatusLabel1",
                schema: "Registry",
                table: "ActivityTypes");

            migrationBuilder.DropColumn(
                name: "StatusLabel2",
                schema: "Registry",
                table: "ActivityTypes");

            migrationBuilder.DropColumn(
                name: "Informations",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                schema: "Docs",
                table: "Activities");
        }
    }
}
