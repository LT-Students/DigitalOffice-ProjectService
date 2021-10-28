using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(ProjectServiceDbContext))]
  [Migration("20211025094030_UpdateTables")]
  public class UpdateTables : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.RenameTable(
        name: "EntitiesImages",
        newName: DbProjectImage.TableName);

      migrationBuilder.DropColumn(
        name: "DepartmentId",
        table: DbProject.TableName);
    }
  }
}
