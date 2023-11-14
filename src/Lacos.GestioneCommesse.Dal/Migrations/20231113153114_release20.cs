using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class release20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Date",
                schema: "Docs",
                table: "PurchaseOrders",
                type: "datetimeoffset(3)",
                precision: 3,
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "Number",
                schema: "Docs",
                table: "PurchaseOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                schema: "Docs",
                table: "PurchaseOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                schema: "Docs",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "Docs",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "Year",
                schema: "Docs",
                table: "PurchaseOrders");
        }
    }
}
