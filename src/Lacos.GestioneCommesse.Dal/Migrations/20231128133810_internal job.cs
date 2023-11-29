using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class internaljob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActivityAttachments_ActivityId",
                schema: "Docs",
                table: "ActivityAttachments");

            migrationBuilder.AddColumn<bool>(
                name: "IsInternalJob",
                schema: "Docs",
                table: "Jobs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityAttachments_ActivityId",
                schema: "Docs",
                table: "ActivityAttachments",
                column: "ActivityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActivityAttachments_ActivityId",
                schema: "Docs",
                table: "ActivityAttachments");

            migrationBuilder.DropColumn(
                name: "IsInternalJob",
                schema: "Docs",
                table: "Jobs");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityAttachments_ActivityId",
                schema: "Docs",
                table: "ActivityAttachments",
                column: "ActivityId");
        }
    }
}
