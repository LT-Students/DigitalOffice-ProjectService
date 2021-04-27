using System;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20210424205311_AddDbTasksTable")]
    public class _20210424205311_AddDbTasksTable : Migration
    {
        private void CreateTableTasks(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbTask.TableName,
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ProjectId = table.Column<Guid>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    AuthorId = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    AssignedTo = table.Column<Guid>(nullable: true),
                    PriorityId = table.Column<Guid>(nullable: false),
                    StatusId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    PlannedMinutes = table.Column<int>(nullable: true),
                    Number = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Tasks", x => x.Id); });
        }

        private void CreateTableTaskProperties(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbTaskProperty.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_TaskProperties", x => x.Id); });
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            CreateTableTasks(migrationBuilder);
            CreateTableTaskProperties(migrationBuilder);
        }
    }
}