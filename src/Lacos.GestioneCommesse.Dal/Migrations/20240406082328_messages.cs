using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class messages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "Docs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset(3)", precision: 3, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatorId = table.Column<long>(type: "bigint", nullable: false),
                    JobId = table.Column<long>(type: "bigint", nullable: true),
                    ActivityId = table.Column<long>(type: "bigint", nullable: true),
                    TicketId = table.Column<long>(type: "bigint", nullable: true),
                    PurchaseOrderId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(3)", precision: 3, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    EditedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(3)", precision: 3, nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedById = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(3)", precision: 3, nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "Docs",
                        principalTable: "Activities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_Jobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "Docs",
                        principalTable: "Jobs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalSchema: "Registry",
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "Docs",
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "Docs",
                        principalTable: "Tickets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessageNotfications",
                schema: "Docs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<long>(type: "bigint", nullable: false),
                    OperatorId = table.Column<long>(type: "bigint", nullable: false),
                    ReadDate = table.Column<DateTimeOffset>(type: "datetimeoffset(3)", precision: 3, nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(3)", precision: 3, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    EditedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(3)", precision: 3, nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedById = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(3)", precision: 3, nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageNotfications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageNotfications_Messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "Docs",
                        principalTable: "Messages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MessageNotfications_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalSchema: "Registry",
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageNotfications_MessageId",
                schema: "Docs",
                table: "MessageNotfications",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageNotfications_OperatorId",
                schema: "Docs",
                table: "MessageNotfications",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ActivityId",
                schema: "Docs",
                table: "Messages",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_JobId",
                schema: "Docs",
                table: "Messages",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_OperatorId",
                schema: "Docs",
                table: "Messages",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_PurchaseOrderId",
                schema: "Docs",
                table: "Messages",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TicketId",
                schema: "Docs",
                table: "Messages",
                column: "TicketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageNotfications",
                schema: "Docs");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "Docs");
        }
    }
}
