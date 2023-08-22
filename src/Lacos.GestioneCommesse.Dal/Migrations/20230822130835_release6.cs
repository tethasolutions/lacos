using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class release6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Constructor",
                schema: "Registry",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasPushBar",
                schema: "Registry",
                table: "Products",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                schema: "Registry",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDoors",
                schema: "Registry",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "REIType",
                schema: "Registry",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                schema: "Registry",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VOCType",
                schema: "Registry",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                schema: "Registry",
                table: "Products",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Constructor",
                schema: "Registry",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasPushBar",
                schema: "Registry",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Location",
                schema: "Registry",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NumberOfDoors",
                schema: "Registry",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "REIType",
                schema: "Registry",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                schema: "Registry",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VOCType",
                schema: "Registry",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Year",
                schema: "Registry",
                table: "Products");
        }
    }
}
