using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class modifiche12dic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerSignatureName",
                schema: "Docs",
                table: "Interventions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentFileName",
                schema: "Docs",
                table: "InterventionProductCheckListItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartDate",
                schema: "Docs",
                table: "Activities",
                type: "datetimeoffset(3)",
                precision: 3,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerSignatureName",
                schema: "Docs",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "AttachmentFileName",
                schema: "Docs",
                table: "InterventionProductCheckListItems");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "Docs",
                table: "Activities");
        }
    }
}
