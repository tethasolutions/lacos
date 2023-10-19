using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class release12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterventionProductCheckListItem_InterventionProductCheckLists_CheckListId",
                table: "InterventionProductCheckListItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InterventionProductCheckListItem_Operators_OperatorId",
                table: "InterventionProductCheckListItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InterventionProductCheckListItem",
                table: "InterventionProductCheckListItem");

            migrationBuilder.RenameTable(
                name: "InterventionProductCheckListItem",
                newName: "InterventionProductCheckListItems",
                newSchema: "Docs");

            migrationBuilder.RenameIndex(
                name: "IX_InterventionProductCheckListItem_OperatorId",
                schema: "Docs",
                table: "InterventionProductCheckListItems",
                newName: "IX_InterventionProductCheckListItems_OperatorId");

            migrationBuilder.RenameIndex(
                name: "IX_InterventionProductCheckListItem_CheckListId",
                schema: "Docs",
                table: "InterventionProductCheckListItems",
                newName: "IX_InterventionProductCheckListItems_CheckListId");

            migrationBuilder.AddColumn<string>(
                name: "ColorHex",
                schema: "Registry",
                table: "ActivityTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInternal",
                schema: "Registry",
                table: "ActivityTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EditedOn",
                schema: "Docs",
                table: "InterventionProductCheckListItems",
                type: "datetimeoffset(3)",
                precision: 3,
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Docs",
                table: "InterventionProductCheckListItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedOn",
                schema: "Docs",
                table: "InterventionProductCheckListItems",
                type: "datetimeoffset(3)",
                precision: 3,
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                schema: "Docs",
                table: "InterventionProductCheckListItems",
                type: "datetimeoffset(3)",
                precision: 3,
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InterventionProductCheckListItems",
                schema: "Docs",
                table: "InterventionProductCheckListItems",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OperatorsActivityTypes",
                schema: "Registry",
                columns: table => new
                {
                    ActivityTypesId = table.Column<long>(type: "bigint", nullable: false),
                    OperatorsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatorsActivityTypes", x => new { x.ActivityTypesId, x.OperatorsId });
                    table.ForeignKey(
                        name: "FK_OperatorsActivityTypes_ActivityTypes_ActivityTypesId",
                        column: x => x.ActivityTypesId,
                        principalSchema: "Registry",
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperatorsActivityTypes_Operators_OperatorsId",
                        column: x => x.OperatorsId,
                        principalSchema: "Registry",
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperatorsActivityTypes_OperatorsId",
                schema: "Registry",
                table: "OperatorsActivityTypes",
                column: "OperatorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_InterventionProductCheckListItems_InterventionProductCheckLists_CheckListId",
                schema: "Docs",
                table: "InterventionProductCheckListItems",
                column: "CheckListId",
                principalSchema: "Docs",
                principalTable: "InterventionProductCheckLists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InterventionProductCheckListItems_Operators_OperatorId",
                schema: "Docs",
                table: "InterventionProductCheckListItems",
                column: "OperatorId",
                principalSchema: "Registry",
                principalTable: "Operators",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterventionProductCheckListItems_InterventionProductCheckLists_CheckListId",
                schema: "Docs",
                table: "InterventionProductCheckListItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InterventionProductCheckListItems_Operators_OperatorId",
                schema: "Docs",
                table: "InterventionProductCheckListItems");

            migrationBuilder.DropTable(
                name: "OperatorsActivityTypes",
                schema: "Registry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InterventionProductCheckListItems",
                schema: "Docs",
                table: "InterventionProductCheckListItems");

            migrationBuilder.DropColumn(
                name: "ColorHex",
                schema: "Registry",
                table: "ActivityTypes");

            migrationBuilder.DropColumn(
                name: "IsInternal",
                schema: "Registry",
                table: "ActivityTypes");

            migrationBuilder.RenameTable(
                name: "InterventionProductCheckListItems",
                schema: "Docs",
                newName: "InterventionProductCheckListItem");

            migrationBuilder.RenameIndex(
                name: "IX_InterventionProductCheckListItems_OperatorId",
                table: "InterventionProductCheckListItem",
                newName: "IX_InterventionProductCheckListItem_OperatorId");

            migrationBuilder.RenameIndex(
                name: "IX_InterventionProductCheckListItems_CheckListId",
                table: "InterventionProductCheckListItem",
                newName: "IX_InterventionProductCheckListItem_CheckListId");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EditedOn",
                table: "InterventionProductCheckListItem",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset(3)",
                oldPrecision: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "InterventionProductCheckListItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedOn",
                table: "InterventionProductCheckListItem",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset(3)",
                oldPrecision: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "InterventionProductCheckListItem",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset(3)",
                oldPrecision: 3);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InterventionProductCheckListItem",
                table: "InterventionProductCheckListItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InterventionProductCheckListItem_InterventionProductCheckLists_CheckListId",
                table: "InterventionProductCheckListItem",
                column: "CheckListId",
                principalSchema: "Docs",
                principalTable: "InterventionProductCheckLists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InterventionProductCheckListItem_Operators_OperatorId",
                table: "InterventionProductCheckListItem",
                column: "OperatorId",
                principalSchema: "Registry",
                principalTable: "Operators",
                principalColumn: "Id");
        }
    }
}
