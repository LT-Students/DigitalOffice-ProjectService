using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20210521003534_AddDefaultTaskAndTaskPropertyInfo")]
    public class AddAdminCommunications : Migration
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
                    nameof(DbTaskProperty.Type),
                    nameof(DbTaskProperty.Description),
                    nameof(DbTaskProperty.CreatedAt),
                    nameof(DbTaskProperty.IsActive)
                },
                columnTypes: new string[]
                {
                    "uniqueidentifier",
                    "nvarchar(max)",
                    "uniqueidentifier",
                    "uniqueidentifier",
                    "int",
                    "nvarchar(max)",
                    "DateTime",
                    "bit"
                },
                new object[]
                {
                    Guid.NewGuid(),
                    "normal",
                    null,
                    null,
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
                    nameof(DbTaskProperty.Type),
                    nameof(DbTaskProperty.Description),
                    nameof(DbTaskProperty.CreatedAt),
                    nameof(DbTaskProperty.IsActive)
                },
                columnTypes: new[]
                {
                    "uniqueidentifier",
                    "nvarchar(max)",
                    "uniqueidentifier",
                    "uniqueidentifier",
                    "int",
                    "nvarchar(max)",
                    "DateTime",
                    "bit"
                },
                new object[]
                {
                    Guid.NewGuid(),
                    "new",
                    null,
                    null,
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
                    nameof(DbTaskProperty.Type),
                    nameof(DbTaskProperty.Description),
                    nameof(DbTaskProperty.CreatedAt),
                    nameof(DbTaskProperty.IsActive)
                },
                columnTypes: new[]
                {
                    "uniqueidentifier",
                    "nvarchar(max)",
                    "uniqueidentifier",
                    "uniqueidentifier",
                    "int",
                    "nvarchar(max)",
                    "DateTime",
                    "bit"
                },
                new object[]
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
        }
    }
}
