using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class dipendenzeattività : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasDependencies",
                schema: "Registry",
                table: "ActivityTypes",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActivityDependencies",
                schema: "Docs",
                columns: table => new
                {
                    ActivityDependenciesId = table.Column<long>(type: "bigint", nullable: false),
                    ParentActivitiesId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityDependencies", x => new { x.ActivityDependenciesId, x.ParentActivitiesId });
                    table.ForeignKey(
                        name: "FK_ActivityDependencies_Activities_ActivityDependenciesId",
                        column: x => x.ActivityDependenciesId,
                        principalSchema: "Docs",
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityDependencies_Activities_ParentActivitiesId",
                        column: x => x.ParentActivitiesId,
                        principalSchema: "Docs",
                        principalTable: "Activities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderDependencies",
                schema: "Docs",
                columns: table => new
                {
                    ParentActivitiesId = table.Column<long>(type: "bigint", nullable: false),
                    PurchaseOrderDependenciesId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderDependencies", x => new { x.ParentActivitiesId, x.PurchaseOrderDependenciesId });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDependencies_Activities_ParentActivitiesId",
                        column: x => x.ParentActivitiesId,
                        principalSchema: "Docs",
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDependencies_PurchaseOrders_PurchaseOrderDependenciesId",
                        column: x => x.PurchaseOrderDependenciesId,
                        principalSchema: "Docs",
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDependencies_ParentActivitiesId",
                schema: "Docs",
                table: "ActivityDependencies",
                column: "ParentActivitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDependencies_PurchaseOrderDependenciesId",
                schema: "Docs",
                table: "PurchaseOrderDependencies",
                column: "PurchaseOrderDependenciesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityDependencies",
                schema: "Docs");

            migrationBuilder.DropTable(
                name: "PurchaseOrderDependencies",
                schema: "Docs");

            migrationBuilder.DropColumn(
                name: "HasDependencies",
                schema: "Registry",
                table: "ActivityTypes");
        }
    }
}
