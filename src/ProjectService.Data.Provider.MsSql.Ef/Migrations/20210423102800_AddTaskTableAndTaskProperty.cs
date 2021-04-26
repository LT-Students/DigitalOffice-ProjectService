﻿using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20210423102800_AddTaskAndTaskPropertyTables")]
    public class AddDbTask : Migration
    {
        private const string ColumnIdName = "Id";

        private void CreateTaskTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbTask.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    AssignedTo = table.Column<Guid?>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    StatusId = table.Column<Guid>(nullable: false),
                    PriorityId = table.Column<Guid>(nullable: false),
                    Deadline = table.Column<DateTime?>(nullable: false),
                    PlannedMinutes = table.Column<int?>(nullable: false),
                    ParentTaskId = table.Column<Guid?>(nullable: false),
                    AuthorId = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Number = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);

                    table.ForeignKey(
                        name: "FK_Tasks_Projects",
                        column: a => a.ProjectId,
                        principalTable: DbProject.TableName,
                        principalColumn: ColumnIdName,
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_Tasks_ProjectUsers",
                        column: a => a.AuthorId,
                        principalTable: DbProjectUser.TableName,
                        principalColumn: ColumnIdName,
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_Tasks_TaskProperties",
                        column: a => a.TypeId,
                        principalTable: DbTaskProperty.TableName,
                        principalColumn: ColumnIdName,
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                       name: "FK_Tasks_TaskProperties",
                       column: a => a.StatusId,
                       principalTable: DbTaskProperty.TableName,
                       principalColumn: ColumnIdName,
                       onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                       name: "FK_Tasks_TaskProperties",
                       column: a => a.PriorityId,
                       principalTable: DbTaskProperty.TableName,
                       principalColumn: ColumnIdName,
                       onDelete: ReferentialAction.Cascade);
                });
        }

        private void CreateTaskPropertyTable(MigrationBuilder migrationBuilder)
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
                   table.PrimaryKey("PK_TaskProperty", x => x.Id);

                   table.ForeignKey(
                       name: "FK_TaskPrtoperty_Projects",
                       column: a => a.ProjectId,
                       principalTable: DbProject.TableName,
                       principalColumn: ColumnIdName,
                       onDelete: ReferentialAction.Cascade);

                   table.ForeignKey(
                       name: "FK_TaskPrtoperty_ProjectUsers",
                       column: a => a.AuthorId,
                       principalTable: DbProjectUser.TableName,
                       principalColumn: ColumnIdName,
                       onDelete: ReferentialAction.Cascade);
               });
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            CreateTaskTable(migrationBuilder);
            CreateTaskPropertyTable(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(DbTask.TableName);
            migrationBuilder.DropTable(DbTaskProperty.TableName);
        }

    }
}
