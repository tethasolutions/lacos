using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class addglobalsettingsvaluestring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                schema: "Registry",
                table: "GlobalSettings",
                newName: "ValueNumber");

            migrationBuilder.AddColumn<string>(
                name: "ValueString",
                schema: "Registry",
                table: "GlobalSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValueString",
                schema: "Registry",
                table: "GlobalSettings");

            migrationBuilder.RenameColumn(
                name: "ValueNumber",
                schema: "Registry",
                table: "GlobalSettings",
                newName: "Value");
        }
    }
}
