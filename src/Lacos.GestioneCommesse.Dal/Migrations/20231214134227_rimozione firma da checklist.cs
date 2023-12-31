﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class rimozionefirmadachecklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerSignatureFileName",
                schema: "Docs",
                table: "InterventionProductCheckLists");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerSignatureFileName",
                schema: "Docs",
                table: "InterventionProductCheckLists",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
