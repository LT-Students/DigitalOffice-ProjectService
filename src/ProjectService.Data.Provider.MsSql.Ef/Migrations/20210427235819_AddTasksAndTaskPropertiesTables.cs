﻿using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20210427235819_AddTasksAndTaskPropertiesTables")]
    class AddTasksAndTaskPropertiesTables : Migration
    {
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
                    Description = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey($"PK_{DbTask.TableName}", x => x.Id);
                });
        }

        private void CreateTaskPropertiesTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: DbTaskProperty.TableName,
               columns: table => new
               {
                   Id = table.Column<Guid>(nullable: false),
                   Name = table.Column<string>(nullable: false),
                   ProjectId = table.Column<Guid?>(nullable: true),
                   AuthorId = table.Column<Guid?>(nullable: true),
                   PropertyType = table.Column<int>(nullable: false),
                   Description = table.Column<string>(nullable: true),
                   CreatedAt = table.Column<DateTime>(nullable: false),
                   IsActive = table.Column<bool>(nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey($"PK_{DbTaskProperty.TableName}", x => x.Id);
               });

            migrationBuilder.InsertData(
                table: DbTaskProperty.TableName,
                columns: new[]
                {
                    nameof(DbTaskProperty.Id),
                    nameof(DbTaskProperty.Name),
                    nameof(DbTaskProperty.PropertyType),
                    nameof(DbTaskProperty.CreatedAt),
                    nameof(DbTaskProperty.IsActive)
                },
                columnTypes: new[]
                {
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                },
                new object[]
                {
                    Guid.NewGuid(),
                    "(normal)",
                    (int)TaskPropertyType.Priority,
                    DateTime.UtcNow,
                    true
                });

            migrationBuilder.InsertData(
                table: DbTaskProperty.TableName,
                columns: new[]
                {
                    nameof(DbTaskProperty.Id),
                    nameof(DbTaskProperty.Name),
                    nameof(DbTaskProperty.PropertyType),
                    nameof(DbTaskProperty.CreatedAt),
                    nameof(DbTaskProperty.IsActive)
                },
                columnTypes: new[]
                {
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                },
                new object[]
                {
                    Guid.NewGuid(),
                    "(new)",
                    (int)TaskPropertyType.Status,
                    DateTime.UtcNow,
                    true
                });

            migrationBuilder.InsertData(
                table: DbTaskProperty.TableName,
                columns: new[]
                {
                    nameof(DbTaskProperty.Id),
                    nameof(DbTaskProperty.Name),
                    nameof(DbTaskProperty.PropertyType),
                    nameof(DbTaskProperty.CreatedAt),
                    nameof(DbTaskProperty.IsActive)
                },
                columnTypes: new[]
                {
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                },
                new object[]
                {
                    Guid.NewGuid(),
                    "(feature)",
                    (int)TaskPropertyType.Type,
                    DateTime.UtcNow,
                    true
                });
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            CreateTaskPropertiesTable(migrationBuilder);
            CreateTasksTable(migrationBuilder);
        }
    }
}
