using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class release1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Operators_UserId",
                schema: "Registry",
                table: "Operators");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                schema: "Registry",
                table: "Operators",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_Operators_UserId",
                schema: "Registry",
                table: "Operators",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Operators_UserId",
                schema: "Registry",
                table: "Operators");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                schema: "Registry",
                table: "Operators",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operators_UserId",
                schema: "Registry",
                table: "Operators",
                column: "UserId",
                unique: true);
        }
    }
}
