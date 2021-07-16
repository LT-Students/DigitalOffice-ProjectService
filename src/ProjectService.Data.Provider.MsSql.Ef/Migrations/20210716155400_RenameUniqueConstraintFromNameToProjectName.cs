using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20210716155400_RenameUniqueConstraintFromNameToProjectName")]
    public class RenameUniqueConstraintFromNameToProjectName : Migration
    {
        protected override void Up(MigrationBuilder builder)
        {
            builder.DropUniqueConstraint(
                name: $"UX_Name_unique",
                table: DbProject.TableName);

            builder.AddUniqueConstraint(
                name: $"UX_ProjectName_unique",
                table: DbProject.TableName,
                column: nameof(DbProject.Name));
        }
    }
}
