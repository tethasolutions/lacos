using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class nuovicampiticket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerSignatureFileName",
                schema: "Docs",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSignatureName",
                schema: "Docs",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportFileName",
                schema: "Docs",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReportGeneratedOn",
                schema: "Docs",
                table: "Tickets",
                type: "datetimeoffset(3)",
                precision: 3,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerSignatureFileName",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CustomerSignatureName",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ReportFileName",
                schema: "Docs",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ReportGeneratedOn",
                schema: "Docs",
                table: "Tickets");
        }
    }
}
