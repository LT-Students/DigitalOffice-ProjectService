using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Database.Migrations
{
    public partial class PublicBoolProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "DbProjectWorkerUser",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsManager",
                table: "DbProjectWorkerUser",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "DbProjectWorkerUser");

            migrationBuilder.DropColumn(
                name: "IsManager",
                table: "DbProjectWorkerUser");
        }
    }
}
