using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.ProjectService.Database.Migrations
{
    public partial class RemovePlural : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectManagersUsers");

            migrationBuilder.DropTable(
                name: "ProjectWorkersUsers");

            migrationBuilder.CreateTable(
                name: "ProjectManagerUser",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(nullable: false),
                    ManagerUserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectManagerUser", x => new { x.ProjectId, x.ManagerUserId });
                    table.ForeignKey(
                        name: "FK_ProjectManagerUser_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectWorkerUser",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(nullable: false),
                    WorkerUserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectWorkerUser", x => new { x.ProjectId, x.WorkerUserId });
                    table.ForeignKey(
                        name: "FK_ProjectWorkerUser_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectManagerUser");

            migrationBuilder.DropTable(
                name: "ProjectWorkerUser");

            migrationBuilder.CreateTable(
                name: "ProjectManagersUsers",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManagerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectManagersUsers", x => new { x.ProjectId, x.ManagerUserId });
                    table.ForeignKey(
                        name: "FK_ProjectManagersUsers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectWorkersUsers",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectWorkersUsers", x => new { x.ProjectId, x.WorkerUserId });
                    table.ForeignKey(
                        name: "FK_ProjectWorkersUsers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
