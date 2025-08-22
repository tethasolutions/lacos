using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class addjobaccountingsandco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobAttachments_Jobs_JobId",
                schema: "Docs",
                table: "JobAttachments");

            migrationBuilder.DropColumn(
                name: "HasQuotation",
                schema: "Registry",
                table: "ActivityTypes");

            migrationBuilder.DropColumn(
                name: "QuotationAmount",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.CreateTable(
                name: "AccountingTypes",
                schema: "Registry",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenerateAlert = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_AccountingTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobAccountings",
                schema: "Docs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<long>(type: "bigint", nullable: false),
                    AccountingTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_JobAccountings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobAccountings_AccountingTypes_AccountingTypeId",
                        column: x => x.AccountingTypeId,
                        principalSchema: "Registry",
                        principalTable: "AccountingTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobAccountings_Jobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "Docs",
                        principalTable: "Jobs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobAccountings_AccountingTypeId",
                schema: "Docs",
                table: "JobAccountings",
                column: "AccountingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobAccountings_JobId",
                schema: "Docs",
                table: "JobAccountings",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobAttachments_Jobs_JobId",
                schema: "Docs",
                table: "JobAttachments",
                column: "JobId",
                principalSchema: "Docs",
                principalTable: "Jobs",
                principalColumn: "Id");

            migrationBuilder.Sql($"INSERT INTO Registry.AccountingTypes (Name,Description,GenerateAlert,CreatedOn,CreatedById,CreatedBy,IsDeleted) " +
                $"VALUES ('Preventivo','Preventivo commessa',0,getdate(),1,'admin',0);" +
                $"INSERT INTO Registry.AccountingTypes (Name,Description,GenerateAlert,CreatedOn,CreatedById,CreatedBy,IsDeleted) " +
                $"VALUES ('Preventivo','Preventivo commessa',0,getdate(),1,'admin',0);" +
                $"INSERT INTO Registry.AccountingTypes (Name,Description,GenerateAlert,CreatedOn,CreatedById,CreatedBy,IsDeleted) " +
                $"VALUES ('Preventivo','Preventivo commessa',0,getdate(),1,'admin',0);" +
                $"INSERT INTO Registry.AccountingTypes (Name,Description,GenerateAlert,CreatedOn,CreatedById,CreatedBy,IsDeleted) " +
                $"VALUES ('Preventivo','Preventivo commessa',0,getdate(),1,'admin',0);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobAttachments_Jobs_JobId",
                schema: "Docs",
                table: "JobAttachments");

            migrationBuilder.DropTable(
                name: "JobAccountings",
                schema: "Docs");

            migrationBuilder.DropTable(
                name: "AccountingTypes",
                schema: "Registry");

            migrationBuilder.AddColumn<bool>(
                name: "HasQuotation",
                schema: "Registry",
                table: "ActivityTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "QuotationAmount",
                schema: "Docs",
                table: "Activities",
                type: "decimal(14,2)",
                precision: 14,
                scale: 2,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobAttachments_Jobs_JobId",
                schema: "Docs",
                table: "JobAttachments",
                column: "JobId",
                principalSchema: "Docs",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
