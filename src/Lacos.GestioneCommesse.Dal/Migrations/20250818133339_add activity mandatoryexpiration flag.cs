using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacos.GestioneCommesse.Dal.Migrations
{
    /// <inheritdoc />
    public partial class addactivitymandatoryexpirationflag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMandatoryExpiration",
                schema: "Docs",
                table: "Activities",
                type: "bit",
                nullable: true);

            migrationBuilder.Sql("UPDATE [Docs].[Activities] SET IsMandatoryExpiration = 0 WHERE IsMandatoryExpiration IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMandatoryExpiration",
                schema: "Docs",
                table: "Activities");
        }
    }
}
