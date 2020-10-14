using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Database.Migrations
{
    public partial class AddShortNameForDbProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectFile");

            migrationBuilder.DropTable(
                name: "ProjectWorkerUser");

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "Project",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectFile",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(nullable: false),
                    FileId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFile", x => new { x.ProjectId, x.FileId });
                    table.ForeignKey(
                        name: "FK_ProjectFile_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectWorkerUser",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(nullable: false),
                    WorkerUserId = table.Column<Guid>(nullable: false),
                    AddedOn = table.Column<DateTime>(nullable: false),
                    RemovedOn = table.Column<DateTime>(nullable: true),
                    IsManager = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectWorkerUser", x => new { x.ProjectId, x.WorkerUserId });
                    table.ForeignKey(
                        name: "FK_ProjectWorkerUser_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectFile");

            migrationBuilder.DropTable(
                name: "ProjectWorkerUser");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "Project");

            migrationBuilder.CreateTable(
                name: "ProjectFile",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFile", x => new { x.ProjectId, x.FileId });
                    table.ForeignKey(
                        name: "FK_ProjectFile_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectWorkerUser",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsManager = table.Column<bool>(type: "bit", nullable: false),
                    RemovedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectWorkerUser", x => new { x.ProjectId, x.WorkerUserId });
                    table.ForeignKey(
                        name: "FK_ProjectWorkerUser_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
