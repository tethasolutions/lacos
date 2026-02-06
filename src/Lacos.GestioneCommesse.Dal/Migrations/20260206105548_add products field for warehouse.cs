using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class addproductsfieldforwarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWarehouseManaged",
                schema: "Registry",
                table: "ProductTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                schema: "Registry",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Material",
                schema: "Registry",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Side",
                schema: "Registry",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                schema: "Registry",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWarehouseManaged",
                schema: "Registry",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "Brand",
                schema: "Registry",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Material",
                schema: "Registry",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Side",
                schema: "Registry",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Size",
                schema: "Registry",
                table: "Products");
        }
    }
}
