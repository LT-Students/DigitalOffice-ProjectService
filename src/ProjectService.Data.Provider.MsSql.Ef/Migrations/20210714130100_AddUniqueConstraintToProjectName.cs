using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20210714130100_AddUniqueConstraintToProjectName")]
    public class AddUniqueConstraintToProjectName : Migration
    {
        protected override void Up(MigrationBuilder builder)
        {
            builder.AlterColumn<string>(
                name: nameof(DbProject.Name),
                table: DbProject.TableName,
                maxLength: 100);

            builder.AddUniqueConstraint(
                name: $"UX_Name_unique",
                table: DbProject.TableName,
                column: nameof(DbProject.Name));
        }
    }
}
