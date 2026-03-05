using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Add_Operator_App_Message_Operator_Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isDefaultAppMessageOperator",
                schema: "Registry",
                table: "Operators",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isOptionalAppMessageOperator",
                schema: "Registry",
                table: "Operators",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefaultAppMessageOperator",
                schema: "Registry",
                table: "Operators");

            migrationBuilder.DropColumn(
                name: "IsOptionalAppMessageOperator",
                schema: "Registry",
                table: "Operators");
        }
    }
}
