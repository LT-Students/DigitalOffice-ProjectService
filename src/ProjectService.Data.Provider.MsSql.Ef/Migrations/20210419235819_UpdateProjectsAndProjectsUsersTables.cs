using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20210419235819_UpdateProjectsAndProjectsUsersTables")]
    class UpdateProjectsAndProjectsUsersTables : Migration
    {
        private void UpdateProjectsTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: DbProject.TableName);

            migrationBuilder.DropColumn(
                name: "ClosedReason",
                table: DbProject.TableName);

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: DbProject.TableName);

            migrationBuilder.AlterColumn<string>(
                name: nameof(DbProject.Name),
                table: DbProject.TableName,
                nullable: false,
                maxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: nameof(DbProject.ShortName),
                table: DbProject.TableName,
                nullable: false,
                maxLength: 30);

            migrationBuilder.AddColumn<string>(
                name: nameof(DbProject.ShortDescription),
                table: DbProject.TableName,
                nullable: true,
                maxLength: 300);

            migrationBuilder.AddColumn<Guid>(
                name: nameof(DbProject.AuthorId),
                table: DbProject.TableName,
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: nameof(DbProject.Status),
                table: DbProject.TableName,
                nullable: false);
        }

        private void UpdateProjectUsersTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: $"FK_ProjectUser_Roles",
                table: DbProjectUser.TableName);

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: DbProjectUser.TableName);

            migrationBuilder.AddColumn<int>(
                name: nameof(DbProjectUser.Role),
                table: DbProjectUser.TableName,
                nullable: false);
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            UpdateProjectsTable(migrationBuilder);
            UpdateProjectUsersTable(migrationBuilder);

            migrationBuilder.DropTable("Roles");
        }
    }
}
