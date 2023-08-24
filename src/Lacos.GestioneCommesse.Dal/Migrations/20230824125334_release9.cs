using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class release9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterventionProducts_Activities_ActivityId",
                schema: "Docs",
                table: "InterventionProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_InterventionProducts_Products_ProductId",
                schema: "Docs",
                table: "InterventionProducts");

            migrationBuilder.DropIndex(
                name: "IX_InterventionProducts_ActivityId",
                schema: "Docs",
                table: "InterventionProducts");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Docs",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                schema: "Docs",
                table: "InterventionProducts");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                schema: "Docs",
                table: "InterventionProducts",
                newName: "ActivityProductId");

            migrationBuilder.RenameIndex(
                name: "IX_InterventionProducts_ProductId",
                schema: "Docs",
                table: "InterventionProducts",
                newName: "IX_InterventionProducts_ActivityProductId");

            migrationBuilder.AlterColumn<long>(
                name: "ActivityId",
                schema: "Docs",
                table: "Interventions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

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

            migrationBuilder.CreateTable(
                name: "ActivityProducts",
                schema: "Docs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ActivityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_ActivityProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityProducts_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "Docs",
                        principalTable: "Activities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Registry",
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityProducts_ActivityId",
                schema: "Docs",
                table: "ActivityProducts",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityProducts_ProductId",
                schema: "Docs",
                table: "ActivityProducts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_InterventionProducts_ActivityProducts_ActivityProductId",
                schema: "Docs",
                table: "InterventionProducts",
                column: "ActivityProductId",
                principalSchema: "Docs",
                principalTable: "ActivityProducts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterventionProducts_ActivityProducts_ActivityProductId",
                schema: "Docs",
                table: "InterventionProducts");

            migrationBuilder.DropTable(
                name: "ActivityProducts",
                schema: "Docs");

            migrationBuilder.RenameColumn(
                name: "ActivityProductId",
                schema: "Docs",
                table: "InterventionProducts",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_InterventionProducts_ActivityProductId",
                schema: "Docs",
                table: "InterventionProducts",
                newName: "IX_InterventionProducts_ProductId");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Docs",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "ActivityId",
                schema: "Docs",
                table: "Interventions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

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

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Docs",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddForeignKey(
                name: "FK_InterventionProducts_Products_ProductId",
                schema: "Docs",
                table: "InterventionProducts",
                column: "ProductId",
                principalSchema: "Registry",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
