using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class release14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Customers_CustomerId",
                schema: "Docs",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Interventions_InterventionId",
                schema: "Docs",
                table: "PurchaseOrders");

            migrationBuilder.RenameColumn(
                name: "InterventionId",
                schema: "Docs",
                table: "PurchaseOrders",
                newName: "JobId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                schema: "Docs",
                table: "PurchaseOrders",
                newName: "SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrders_InterventionId",
                schema: "Docs",
                table: "PurchaseOrders",
                newName: "IX_PurchaseOrders_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrders_CustomerId",
                schema: "Docs",
                table: "PurchaseOrders",
                newName: "IX_PurchaseOrders_SupplierId");

            migrationBuilder.CreateTable(
                name: "Suppliers",
                schema: "Registry",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupplierAddresses",
                schema: "Registry",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsMainAddress = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_SupplierAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierAddresses_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "Registry",
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierAddresses_SupplierId",
                schema: "Registry",
                table: "SupplierAddresses",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Jobs_JobId",
                schema: "Docs",
                table: "PurchaseOrders",
                column: "JobId",
                principalSchema: "Docs",
                principalTable: "Jobs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Suppliers_SupplierId",
                schema: "Docs",
                table: "PurchaseOrders",
                column: "SupplierId",
                principalSchema: "Registry",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Jobs_JobId",
                schema: "Docs",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Suppliers_SupplierId",
                schema: "Docs",
                table: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "SupplierAddresses",
                schema: "Registry");

            migrationBuilder.DropTable(
                name: "Suppliers",
                schema: "Registry");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                schema: "Docs",
                table: "PurchaseOrders",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "JobId",
                schema: "Docs",
                table: "PurchaseOrders",
                newName: "InterventionId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrders_SupplierId",
                schema: "Docs",
                table: "PurchaseOrders",
                newName: "IX_PurchaseOrders_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrders_JobId",
                schema: "Docs",
                table: "PurchaseOrders",
                newName: "IX_PurchaseOrders_InterventionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Customers_CustomerId",
                schema: "Docs",
                table: "PurchaseOrders",
                column: "CustomerId",
                principalSchema: "Registry",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Interventions_InterventionId",
                schema: "Docs",
                table: "PurchaseOrders",
                column: "InterventionId",
                principalSchema: "Docs",
                principalTable: "Interventions",
                principalColumn: "Id");
        }
    }
}
