using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20210521003534_AddDefaultTaskAndTaskPropertyInfo")]
    public class AddDefaultTaskAndTaskPropertyInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: DbTaskProperty.TableName,
                columns: new[]
                {
                    nameof(DbTaskProperty.Id),
                    nameof(DbTaskProperty.Name),
                    nameof(DbTaskProperty.ProjectId),
                    nameof(DbTaskProperty.AuthorId),
                    nameof(DbTaskProperty.PropertyType),
                    nameof(DbTaskProperty.Description),
                    nameof(DbTaskProperty.CreatedAt),
                    nameof(DbTaskProperty.IsActive)
                },
                columnTypes: new []
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
                    "normal",
                    Guid.Empty,
                    Guid.Empty,
                    (int)TaskPropertyType.Priority,
                    null,
                    DateTime.UtcNow,
                    true
                });

            migrationBuilder.InsertData(
            table: DbTaskProperty.TableName,
                columns: new[]
                {
                    nameof(DbTaskProperty.Id),
                    nameof(DbTaskProperty.Name),
                    nameof(DbTaskProperty.ProjectId),
                    nameof(DbTaskProperty.AuthorId),
                    nameof(DbTaskProperty.PropertyType),
                    nameof(DbTaskProperty.Description),
                    nameof(DbTaskProperty.CreatedAt),
                    nameof(DbTaskProperty.IsActive)
                },
                columnTypes: new []
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
                    Guid.Empty,
                    Guid.Empty,
                    (int)TaskPropertyType.Status,
                    null,
                    DateTime.UtcNow,
                    true
                });

                migrationBuilder.InsertData(
                table: DbTaskProperty.TableName,
                columns: new[]
                {
                    nameof(DbTaskProperty.Id),
                    nameof(DbTaskProperty.Name),
                    nameof(DbTaskProperty.ProjectId),
                    nameof(DbTaskProperty.AuthorId),
                    nameof(DbTaskProperty.PropertyType),
                    nameof(DbTaskProperty.Description),
                    nameof(DbTaskProperty.CreatedAt),
                    nameof(DbTaskProperty.IsActive)
                },
                columnTypes: new []
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
                    Guid.Empty,
                    Guid.Empty,
                    (int)TaskPropertyType.Type,
                    null,
                    DateTime.UtcNow,
                    true
                });
        }
    }
}
