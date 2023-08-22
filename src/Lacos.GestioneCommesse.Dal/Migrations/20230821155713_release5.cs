using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class release5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalFilename",
                schema: "Registry",
                table: "ProductDocuments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<long>(
                name: "InterventionId",
                schema: "Docs",
                table: "InterventionProducts",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "ActivityId",
                schema: "Docs",
                table: "InterventionProducts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_InterventionProducts_ActivityId",
                schema: "Docs",
                table: "InterventionProducts",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_InterventionProducts_Activities_ActivityId",
                schema: "Docs",
                table: "InterventionProducts",
                column: "ActivityId",
                principalSchema: "Docs",
                principalTable: "Activities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterventionProducts_Activities_ActivityId",
                schema: "Docs",
                table: "InterventionProducts");

            migrationBuilder.DropIndex(
                name: "IX_InterventionProducts_ActivityId",
                schema: "Docs",
                table: "InterventionProducts");

            migrationBuilder.DropColumn(
                name: "OriginalFilename",
                schema: "Registry",
                table: "ProductDocuments");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                schema: "Docs",
                table: "InterventionProducts");

            migrationBuilder.AlterColumn<long>(
                name: "InterventionId",
                schema: "Docs",
                table: "InterventionProducts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
