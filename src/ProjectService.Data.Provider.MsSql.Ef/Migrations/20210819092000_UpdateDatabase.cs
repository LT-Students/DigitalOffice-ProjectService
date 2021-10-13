using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20210819092000_UpdateDatabase")]
    public class UpdateDatabase : Migration
    {
        private void AddProjectsTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbProject.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DepartmentId = table.Column<Guid>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false, maxLength: 150),
                    ShortName = table.Column<string>(nullable: true, maxLength: 30),
                    Description = table.Column<string>(nullable: true),
                    ShortDescription = table.Column<string>(nullable: true, maxLength: 300),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<Guid?>(nullable: true),
                    ModifiedAtUtc = table.Column<DateTime?>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.UniqueConstraint("UX_Project_Name_Unique", x => x.Name);
                });
        }

        private void AddProjectFilesTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbProjectFile.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    FileId = table.Column<Guid>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectsFiles", x => x.Id);
                });
        }

        private void AddProjectUsersTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbProjectUser.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<Guid?>(nullable: true),
                    ModifiedAtUtc = table.Column<DateTime?>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectsUsers", x => x.Id);
                });
        }

        private void AddTasksTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    AssignedTo = table.Column<Guid?>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false),
                    StatusId = table.Column<Guid>(nullable: false),
                    PriorityId = table.Column<Guid>(nullable: false),
                    PlannedMinutes = table.Column<int?>(nullable: true),
                    ParentId = table.Column<Guid?>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<Guid?>(nullable: true),
                    ModifiedAtUtc = table.Column<DateTime?>(nullable: true),
                    Number = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });
        }

        private void AddTaskPropertiesTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ProjectId = table.Column<Guid?>(nullable: true),
                    PropertyType = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<Guid?>(nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<Guid?>(nullable: true),
                    ModifiedAtUtc = table.Column<DateTime?>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskProperties", x => x.Id);
                });
        }

        private void AddDefaultTypes(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TaskProperties",
                columns: new[]
                {
                    "Id",
                    "Name",
                    "ProjectId",
                    "CreatedBy",
                    "PropertyType",
                    "Description",
                    "CreatedAtUtc",
                    "IsActive"
                },
                columnTypes: new[]
                {
                    "uniqueidentifier",
                    "nvarchar(max)",
                    "uniqueidentifier",
                    "uniqueidentifier",
                    "int",
                    "nvarchar(max)",
                    "datetime2",
                    "bit"
                },
                values: new object[]
                {
                    Guid.NewGuid(),
                    "Feature",
                    null,
                    null,
                    (int)TaskPropertyType.Type,
                    null,
                    DateTime.UtcNow,
                    true
                });

            migrationBuilder.InsertData(
                table: "TasksProperties",
                columns: new[]
                {
                    "Id",
                    "Name",
                    "ProjectId",
                    "CreatedBy",
                    "PropertyType",
                    "Description",
                    "CreatedAtUtc",
                    "IsActive"
                },
                columnTypes: new[]
                {
                    "uniqueidentifier",
                    "nvarchar(max)",
                    "uniqueidentifier",
                    "uniqueidentifier",
                    "int",
                    "nvarchar(max)",
                    "datetime2",
                    "bit"
                },
                values: new object[]
                {
                    Guid.NewGuid(),
                    "Bug",
                    null,
                    null,
                    (int)TaskPropertyType.Type,
                    null,
                    DateTime.UtcNow,
                    true
                });

            migrationBuilder.InsertData(
                table: "TasksProperties",
                columns: new[]
                {
                    "Id",
                    "Name",
                    "ProjectId",
                    "CreatedBy",
                    "PropertyType",
                    "Description",
                    "CreatedAtUtc",
                    "IsActive"
                },
                columnTypes: new[]
                {
                    "uniqueidentifier",
                    "nvarchar(max)",
                    "uniqueidentifier",
                    "uniqueidentifier",
                    "int",
                    "nvarchar(max)",
                    "datetime2",
                    "bit"
                },
                values: new object[]
                {
                    Guid.NewGuid(),
                    "Task",
                    null,
                    null,
                    (int)TaskPropertyType.Type,
                    null,
                    DateTime.UtcNow,
                    true
                });
        }

        private void AddDefaultPriorities(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TasksProperties",
                columns: new[]
                {
                    "Id",
                    "Name",
                    "ProjectId",
                    "CreatedBy",
                    "PropertyType",
                    "Description",
                    "CreatedAtUtc",
                    "IsActive"
                },
                columnTypes: new[]
                {
                    "uniqueidentifier",
                    "nvarchar(max)",
                    "uniqueidentifier",
                    "uniqueidentifier",
                    "int",
                    "nvarchar(max)",
                    "datetime2",
                    "bit"
                },
                values: new object[]
                {
                    Guid.NewGuid(),
                    "Normal",
                    null,
                    null,
                    (int)TaskPropertyType.Priority,
                    null,
                    DateTime.UtcNow,
                    true
                });

            migrationBuilder.InsertData(
                table: "TasksProperties",
                columns: new[]
                {
                    "Id",
                    "Name",
                    "ProjectId",
                    "CreatedBy",
                    "PropertyType",
                    "Description",
                    "CreatedAtUtc",
                    "IsActive"
                },
                columnTypes: new[]
                {
                    "uniqueidentifier",
                    "nvarchar(max)",
                    "uniqueidentifier",
                    "uniqueidentifier",
                    "int",
                    "nvarchar(max)",
                    "datetime2",
                    "bit"
                },
                values: new object[]
                {
                    Guid.NewGuid(),
                    "High",
                    null,
                    null,
                    (int)TaskPropertyType.Priority,
                    null,
                    DateTime.UtcNow,
                    true
                });

            migrationBuilder.InsertData(
                table: "TasksProperties",
                columns: new[]
                {
                    "Id",
                    "Name",
                    "ProjectId",
                    "CreatedBy",
                    "PropertyType",
                    "Description",
                    "CreatedAtUtc",
                    "IsActive"
                },
                columnTypes: new[]
                {
                    "uniqueidentifier",
                    "nvarchar(max)",
                    "uniqueidentifier",
                    "uniqueidentifier",
                    "int",
                    "nvarchar(max)",
                    "datetime2",
                    "bit"
                },
                values: new object[]
                {
                    Guid.NewGuid(),
                    "Low",
                    null,
                    null,
                    (int)TaskPropertyType.Priority,
                    null,
                    DateTime.UtcNow,
                    true
                });
        }

        private void AddDefaultStatuses(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "TasksProperties",
                columns: new[]
                {
                    "Id",
                    "Name",
                    "ProjectId",
                    "CreatedBy",
                    "PropertyType",
                    "Description",
                    "CreatedAtUtc",
                    "IsActive"
                },
                columnTypes: new[]
                {
                    "uniqueidentifier",
                    "nvarchar(max)",
                    "uniqueidentifier",
                    "uniqueidentifier",
                    "int",
                    "nvarchar(max)",
                    "datetime2",
                    "bit"
                },
                values: new object[]
                {
                    Guid.NewGuid(),
                    "New",
                    null,
                    null,
                    (int)TaskPropertyType.Status,
                    null,
                    DateTime.UtcNow,
                    true
                });

            migrationBuilder.InsertData(
            table: "TasksProperties",
                columns: new[]
                {
                    "Id",
                    "Name",
                    "ProjectId",
                    "CreatedBy",
                    "PropertyType",
                    "Description",
                    "CreatedAtUtc",
                    "IsActive"
                },
                columnTypes: new[]
                {
                    "uniqueidentifier",
                    "nvarchar(max)",
                    "uniqueidentifier",
                    "uniqueidentifier",
                    "int",
                    "nvarchar(max)",
                    "datetime2",
                    "bit"
                },
                values: new object[]
                {
                    Guid.NewGuid(),
                    "In Progress",
                    null,
                    null,
                    (int)TaskPropertyType.Status,
                    null,
                    DateTime.UtcNow,
                    true
                });

            migrationBuilder.InsertData(
            table: "TasksProperties",
                columns: new[]
                {
                    "Id",
                    "Name",
                    "ProjectId",
                    "CreatedBy",
                    "PropertyType",
                    "Description",
                    "CreatedAtUtc",
                    "IsActive"
                },
                columnTypes: new[]
                {
                    "uniqueidentifier",
                    "nvarchar(max)",
                    "uniqueidentifier",
                    "uniqueidentifier",
                    "int",
                    "nvarchar(max)",
                    "datetime2",
                    "bit"
                },
                values: new object[]
                {
                    Guid.NewGuid(),
                    "Done",
                    null,
                    null,
                    (int)TaskPropertyType.Status,
                    null,
                    DateTime.UtcNow,
                    true
                });
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddProjectsTable(migrationBuilder);

            AddProjectFilesTable(migrationBuilder);

            AddProjectUsersTable(migrationBuilder);

            AddTasksTable(migrationBuilder);

            AddTaskPropertiesTable(migrationBuilder);

            AddDefaultTypes(migrationBuilder);

            AddDefaultPriorities(migrationBuilder);

            AddDefaultStatuses(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(DbProject.TableName);
            migrationBuilder.DropTable(DbProjectFile.TableName);
            migrationBuilder.DropTable(DbProjectUser.TableName);
            migrationBuilder.DropTable("Tasks");
            migrationBuilder.DropTable("TasksProperties");
        }
    }
}
