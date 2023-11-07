using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class release17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Suppliers_SupplierId",
                schema: "Registry",
                table: "Addresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                schema: "Registry",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "Addresses",
                schema: "Registry",
                newName: "Addresses",
                newSchema: "Registry");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_SupplierId",
                schema: "Registry",
                table: "Addresses",
                newName: "IX_Addresses_SupplierId");

            migrationBuilder.AddColumn<long>(
                name: "AddressId",
                schema: "Docs",
                table: "Jobs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AddressId",
                schema: "Docs",
                table: "Activities",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "SupplierId",
                schema: "Registry",
                table: "Addresses",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                schema: "Registry",
                table: "Addresses",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_AddressId",
                schema: "Docs",
                table: "Jobs",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_AddressId",
                schema: "Docs",
                table: "Activities",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Addresses_AddressId",
                schema: "Docs",
                table: "Activities",
                column: "AddressId",
                principalSchema: "Registry",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Suppliers_SupplierId",
                schema: "Registry",
                table: "Addresses",
                column: "SupplierId",
                principalSchema: "Registry",
                principalTable: "Suppliers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Addresses_AddressId",
                schema: "Docs",
                table: "Jobs",
                column: "AddressId",
                principalSchema: "Registry",
                principalTable: "Addresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Addresses_AddressId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Suppliers_SupplierId",
                schema: "Registry",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Addresses_AddressId",
                schema: "Docs",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_AddressId",
                schema: "Docs",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Activities_AddressId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                schema: "Registry",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "AddressId",
                schema: "Docs",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "AddressId",
                schema: "Docs",
                table: "Activities");

            migrationBuilder.RenameTable(
                name: "Addresses",
                schema: "Registry",
                newName: "Addresses",
                newSchema: "Registry");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_SupplierId",
                schema: "Registry",
                table: "Addresses",
                newName: "IX_Addresses_SupplierId");

            migrationBuilder.AlterColumn<long>(
                name: "SupplierId",
                schema: "Registry",
                table: "Addresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                schema: "Registry",
                table: "Addresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Suppliers_SupplierId",
                schema: "Registry",
                table: "Addresses",
                column: "SupplierId",
                principalSchema: "Registry",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }
    }
}
