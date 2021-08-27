using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20210824232700_AddProjectsImagesTable")]
    public class AddProjectsImagesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: DbProjectImage.TableName,
               columns: table => new
               {
                   Id = table.Column<Guid>(nullable: false),
                   ProjectId = table.Column<Guid>(nullable: false),
                   ImageId = table.Column<Guid>(nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_ProjectsImages", x => x.Id);
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(DbProjectImage.TableName);
        }
    }
}
