using System;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Database.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20201027235819_InitialCreate")]
    public class InitialCreate : Migration
    {
        private const string ColumnIdName = "Id";

        #region Create tables

        private void CreateTableProjects(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbProject.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ShortName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DepartmentId = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ClosedAt = table.Column<DateTime>(nullable: true),
                    ClosedReason = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });
        }

        private void CreateTableProjectsFiles(MigrationBuilder migrationBuilder)
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
                    table.PrimaryKey("PK_ProjectFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectFile_Projects",
                        column: x => x.ProjectId,
                        principalTable: DbProject.TableName,
                        principalColumn: ColumnIdName,
                        onDelete: ReferentialAction.Cascade);
                });
        }

        private void CreateTableProjectsUsers(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbProjectUser.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false),
                    AddedOn = table.Column<DateTime>(nullable: false),
                    RemovedOn = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectUser_Projects",
                        column: x => x.ProjectId,
                        principalTable: DbProject.TableName,
                        principalColumn: ColumnIdName,
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUser_Roles",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: ColumnIdName,
                        onDelete: ReferentialAction.Cascade);
                });
        }

        private void CreateTableRoles(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });
        }

        #endregion

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            CreateTableProjects(migrationBuilder);

            CreateTableRoles(migrationBuilder);

            CreateTableProjectsFiles(migrationBuilder);

            CreateTableProjectsUsers(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(DbProjectUser.TableName);

            migrationBuilder.DropTable(DbProjectFile.TableName);

            migrationBuilder.DropTable("Roles");

            migrationBuilder.DropTable(DbProject.TableName);
        }

        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}
