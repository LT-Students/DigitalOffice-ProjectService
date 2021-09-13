using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20210824232700_AddEntitiesImagesTable")]
    public class AddEntitiesImagesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: DbEntityImage.TableName,
               columns: table => new
               {
                   Id = table.Column<Guid>(nullable: false),
                   EntityId = table.Column<Guid>(nullable: false),
                   ImageId = table.Column<Guid>(nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_EntitiesImages", x => x.Id);
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(DbEntityImage.TableName);
        }
    }
}
