using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class release8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VOCType",
                schema: "Registry",
                table: "Products",
                newName: "VocType");

            migrationBuilder.RenameColumn(
                name: "REIType",
                schema: "Registry",
                table: "Products",
                newName: "ReiType");

            migrationBuilder.RenameColumn(
                name: "Constructor",
                schema: "Registry",
                table: "Products",
                newName: "ConstructorName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VocType",
                schema: "Registry",
                table: "Products",
                newName: "VOCType");

            migrationBuilder.RenameColumn(
                name: "ReiType",
                schema: "Registry",
                table: "Products",
                newName: "REIType");

            migrationBuilder.RenameColumn(
                name: "ConstructorName",
                schema: "Registry",
                table: "Products",
                newName: "Constructor");
        }
    }
}
