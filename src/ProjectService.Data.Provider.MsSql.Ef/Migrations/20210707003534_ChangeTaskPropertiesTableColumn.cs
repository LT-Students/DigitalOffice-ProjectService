using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20210707003534_ChangeTaskPropertiesTableColumn")]
    public class ChangeTaskPropertiesTableColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid?>(
                name: nameof(DbTaskProperty.ProjectId),
                table: DbTaskProperty.TableName,
                nullable: true);

            migrationBuilder.AlterColumn<Guid?>(
                name: nameof(DbTaskProperty.AuthorId),
                table: DbTaskProperty.TableName,
                nullable: true);
        }
    }
}
