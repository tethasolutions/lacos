using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class attachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityAttachments_Activities_ActivityId",
                schema: "Docs",
                table: "ActivityAttachments");

            migrationBuilder.DropIndex(
                name: "IX_ActivityAttachments_ActivityId",
                schema: "Docs",
                table: "ActivityAttachments");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityAttachments_ActivityId",
                schema: "Docs",
                table: "ActivityAttachments",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityAttachments_Activities_ActivityId",
                schema: "Docs",
                table: "ActivityAttachments",
                column: "ActivityId",
                principalSchema: "Docs",
                principalTable: "Activities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityAttachments_Activities_ActivityId",
                schema: "Docs",
                table: "ActivityAttachments");

            migrationBuilder.DropIndex(
                name: "IX_ActivityAttachments_ActivityId",
                schema: "Docs",
                table: "ActivityAttachments");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityAttachments_ActivityId",
                schema: "Docs",
                table: "ActivityAttachments",
                column: "ActivityId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityAttachments_Activities_ActivityId",
                schema: "Docs",
                table: "ActivityAttachments",
                column: "ActivityId",
                principalSchema: "Docs",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
