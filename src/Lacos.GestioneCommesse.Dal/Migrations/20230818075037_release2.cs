using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class release2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "Registry",
                table: "OperatorDocuments",
                newName: "OriginalFilename");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "Registry",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                schema: "Registry",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "Registry",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Telephone",
                schema: "Registry",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "OriginalFilename",
                schema: "Registry",
                table: "OperatorDocuments",
                newName: "Description");
        }
    }
}
