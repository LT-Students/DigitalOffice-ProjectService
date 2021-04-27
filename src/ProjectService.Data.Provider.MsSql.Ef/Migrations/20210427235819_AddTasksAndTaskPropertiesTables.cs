using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20210427235819_AddTasksAndTaskPropertiesTables")]
    class AddTasksAndTaskPropertiesTables : Migration
    {
        private const string ColumnIdName = "Id";

        private void CreateTasksTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbTask.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    StatusId = table.Column<Guid>(nullable: false),
                    AuthorId = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    AssignedTo = table.Column<Guid?>(nullable: true),
                    PriorityId = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid?>(nullable: true),
                    Number = table.Column<int>(nullable: false),
                    PlannedMinutes = table.Column<int?>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Deadline = table.Column<DateTime?>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey($"PK_{DbTask.TableName}", x => x.Id);

                    table.ForeignKey(
                        name: $"FK_{DbTask.TableName}_{DbProject.TableName}",
                        column: a => a.ProjectId,
                        principalTable: DbProject.TableName,
                        principalColumn: ColumnIdName,
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: $"FK_{DbTask.TableName}_{DbProjectUser.TableName}",
                        column: a => a.AuthorId,
                        principalTable: DbProjectUser.TableName,
                        principalColumn: ColumnIdName,
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: $"FK_{DbTask.TableName}_{DbProjectUser.TableName}",
                        column: a => a.AssignedTo,
                        principalTable: DbProjectUser.TableName,
                        principalColumn: ColumnIdName,
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: $"FK_{DbTask.TableName}_{DbTaskProperty.TableName}",
                        column: a => a.TypeId,
                        principalTable: DbTaskProperty.TableName,
                        principalColumn: ColumnIdName,
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                       name: $"FK_{DbTask.TableName}_{DbTaskProperty.TableName}",
                       column: a => a.StatusId,
                       principalTable: DbTaskProperty.TableName,
                       principalColumn: ColumnIdName,
                       onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                       name: $"FK_{DbTask.TableName}_{DbTaskProperty.TableName}",
                       column: a => a.PriorityId,
                       principalTable: DbTaskProperty.TableName,
                       principalColumn: ColumnIdName,
                       onDelete: ReferentialAction.Cascade);
                });
        }

        private void CreateTaskProperties(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: DbTaskProperty.TableName,
               columns: table => new
               {
                   Id = table.Column<Guid>(nullable: false),
                   Name = table.Column<string>(nullable: false),
                   ProjectId = table.Column<Guid>(nullable: false),
                   AuthorId = table.Column<Guid>(nullable: false),
                   PropertyType = table.Column<int>(nullable: false),
                   Description = table.Column<string>(nullable: false),
                   CreatedAt = table.Column<DateTime>(nullable: false),
                   IsActive = table.Column<bool>(nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey($"PK_{DbTaskProperty.TableName}", x => x.Id);

                   table.ForeignKey(
                       name: $"FK_{DbTaskProperty.TableName}_{DbProject.TableName}",
                       column: a => a.ProjectId,
                       principalTable: DbProject.TableName,
                       principalColumn: ColumnIdName,
                       onDelete: ReferentialAction.Cascade);

                   table.ForeignKey(
                       name: $"FK_{DbTaskProperty.TableName}_{DbProjectUser.TableName}",
                       column: a => a.AuthorId,
                       principalTable: DbProjectUser.TableName,
                       principalColumn: ColumnIdName,
                       onDelete: ReferentialAction.Cascade);
               });
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            CreateTasksTable(migrationBuilder);
            CreateTaskProperties(migrationBuilder);
        }
    }
}
