using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class release10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCode",
                schema: "Registry",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "QrCodeNumber",
                schema: "Registry",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QrCodePrefix",
                schema: "Registry",
                table: "Products",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCodeNumber",
                schema: "Registry",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "QrCodePrefix",
                schema: "Registry",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "QrCode",
                schema: "Registry",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
