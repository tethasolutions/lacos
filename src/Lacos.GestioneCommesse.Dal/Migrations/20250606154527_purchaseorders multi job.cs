using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class purchaseordersmultijob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseOrdersJobs",
                schema: "Docs",
                columns: table => new
                {
                    JobsId = table.Column<long>(type: "bigint", nullable: false),
                    PurchaseOrdersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrdersJobs", x => new { x.JobsId, x.PurchaseOrdersId });
                    table.ForeignKey(
                        name: "FK_PurchaseOrdersJobs_Jobs_JobsId",
                        column: x => x.JobsId,
                        principalSchema: "Docs",
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrdersJobs_PurchaseOrders_PurchaseOrdersId",
                        column: x => x.PurchaseOrdersId,
                        principalSchema: "Docs",
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrdersJobs_PurchaseOrdersId",
                schema: "Docs",
                table: "PurchaseOrdersJobs",
                column: "PurchaseOrdersId");

            migrationBuilder.Sql("INSERT INTO Docs.PurchaseOrdersJobs (JobsId,PurchaseOrdersId) SELECT JobId,Id FROM Docs.PurchaseOrders WHERE IsDeleted=0 AND JobId IS NOT NULL;");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Jobs_JobId",
                schema: "Docs",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_JobId",
                schema: "Docs",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "JobId",
                schema: "Docs",
                table: "PurchaseOrders");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseOrdersJobs",
                schema: "Docs");

            migrationBuilder.AddColumn<long>(
                name: "JobId",
                schema: "Docs",
                table: "PurchaseOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_JobId",
                schema: "Docs",
                table: "PurchaseOrders",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Jobs_JobId",
                schema: "Docs",
                table: "PurchaseOrders",
                column: "JobId",
                principalSchema: "Docs",
                principalTable: "Jobs",
                principalColumn: "Id");
        }
    }
}
