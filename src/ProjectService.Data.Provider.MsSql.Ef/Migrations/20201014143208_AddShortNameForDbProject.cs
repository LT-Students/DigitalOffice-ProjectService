using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Database.Migrations
{
    public partial class AddShortNameForDbProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "Projects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "Projects");
        }
    }
}
