﻿using System;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(ProjectServiceDbContext))]
  [Migration("20220805121500_InitialTables")]
  public class InitialTables : Migration
  {
    private void AddProjectsTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbProject.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          Status = table.Column<int>(nullable: false),
          Name = table.Column<string>(nullable: false, maxLength: 150),
          ShortName = table.Column<string>(nullable: false, maxLength: 40),
          Description = table.Column<string>(nullable: true),
          ShortDescription = table.Column<string>(nullable: true),
          Customer = table.Column<string>(nullable: true, maxLength: 150),
          StartDateUtc = table.Column<DateTime>(nullable: false),
          EndDateUtc = table.Column<DateTime>(nullable: true),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          ModifiedBy = table.Column<Guid?>(nullable: true),
          ModifiedAtUtc = table.Column<DateTime?>(nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Projects", x => x.Id);
          table.UniqueConstraint("UC_Projects_Name_Unique", x => x.Name);
          table.UniqueConstraint("UC_Projects_ShortName_Unique", x => x.Name);
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
          Access = table.Column<int>(nullable: false),
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
          IsActive = table.Column<bool>(nullable: false),
          PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
            .Annotation("SqlServer:IsTemporal", true)
            .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
            .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
          PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
            .Annotation("SqlServer:IsTemporal", true)
            .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
            .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_ProjectsUsers", x => x.Id);
        })
        .Annotation("SqlServer:IsTemporal", true)
        .Annotation("SqlServer:TemporalHistoryTableName", DbProjectUser.HistoryTableName)
        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");
    }

    private void CreateTableProjectsDepartments(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbProjectDepartment.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          ProjectId = table.Column<Guid>(nullable: false),
          DepartmentId = table.Column<Guid>(nullable: false),
          IsActive = table.Column<bool>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
            .Annotation("SqlServer:IsTemporal", true)
            .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
            .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
          PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
            .Annotation("SqlServer:IsTemporal", true)
            .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
            .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
        },
        constraints: table =>
        {
          table.PrimaryKey($"PK_{DbProjectDepartment.TableName}", x => x.Id);
        })
        .Annotation("SqlServer:IsTemporal", true)
        .Annotation("SqlServer:TemporalHistoryTableName", $"{DbProjectDepartment.TableName}History")
        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");
    }

    private void AddProjectImagesTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbProjectImage.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          ProjectId = table.Column<Guid>(nullable: false),
          ImageId = table.Column<Guid>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_ProjectsImages", x => x.Id);
        });
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
      AddProjectsTable(migrationBuilder);

      AddProjectFilesTable(migrationBuilder);

      AddProjectUsersTable(migrationBuilder);

      AddProjectImagesTable(migrationBuilder);

      CreateTableProjectsDepartments(migrationBuilder);
    }
  }
}
