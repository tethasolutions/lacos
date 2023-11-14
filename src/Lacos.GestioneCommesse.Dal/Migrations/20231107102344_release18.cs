using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class release18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_CustomerAddresses_CustomerAddressId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_CustomerAddresses_CustomerAddressId",
                schema: "Registry",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_CustomerAddresses_CustomerAddressId",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "CustomerAddresses",
                schema: "Registry");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_CustomerAddressId",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Activities_CustomerAddressId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "CustomerAddressId",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CustomerAddressId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "CustomerAddressId",
                schema: "Registry",
                table: "Products",
                newName: "AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CustomerAddressId",
                schema: "Registry",
                table: "Products",
                newName: "IX_Products_AddressId");

            migrationBuilder.AddColumn<long>(
                name: "CustomerId",
                schema: "Registry",
                table: "Addresses",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CustomerId",
                schema: "Registry",
                table: "Addresses",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Customers_CustomerId",
                schema: "Registry",
                table: "Addresses",
                column: "CustomerId",
                principalSchema: "Registry",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Addresses_AddressId",
                schema: "Registry",
                table: "Products",
                column: "AddressId",
                principalSchema: "Registry",
                principalTable: "Addresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Customers_CustomerId",
                schema: "Registry",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Addresses_AddressId",
                schema: "Registry",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_CustomerId",
                schema: "Registry",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "Registry",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                schema: "Registry",
                table: "Products",
                newName: "CustomerAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_AddressId",
                schema: "Registry",
                table: "Products",
                newName: "IX_Products_CustomerAddressId");

            migrationBuilder.AddColumn<long>(
                name: "CustomerAddressId",
                schema: "Docs",
                table: "Tickets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CustomerAddressId",
                schema: "Docs",
                table: "Activities",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerAddresses",
                schema: "Registry",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    City = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(3)", precision: 3, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedById = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(3)", precision: 3, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedById = table.Column<long>(type: "bigint", nullable: true),
                    EditedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(3)", precision: 3, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsMainAddress = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Province = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerAddresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "Registry",
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CustomerAddressId",
                schema: "Docs",
                table: "Tickets",
                column: "CustomerAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CustomerAddressId",
                schema: "Docs",
                table: "Activities",
                column: "CustomerAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_CustomerId",
                schema: "Registry",
                table: "CustomerAddresses",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_CustomerAddresses_CustomerAddressId",
                schema: "Docs",
                table: "Activities",
                column: "CustomerAddressId",
                principalSchema: "Registry",
                principalTable: "CustomerAddresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CustomerAddresses_CustomerAddressId",
                schema: "Registry",
                table: "Products",
                column: "CustomerAddressId",
                principalSchema: "Registry",
                principalTable: "CustomerAddresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_CustomerAddresses_CustomerAddressId",
                schema: "Docs",
                table: "Tickets",
                column: "CustomerAddressId",
                principalSchema: "Registry",
                principalTable: "CustomerAddresses",
                principalColumn: "Id");
        }
    }
}
