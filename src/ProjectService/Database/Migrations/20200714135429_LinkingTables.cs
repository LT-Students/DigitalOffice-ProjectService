using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.ProjectService.Database.Migrations
{
    public partial class LinkingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectManagersUsers",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(nullable: false),
                    ManagerUserId = table.Column<Guid>(nullable: false)
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
                    ProjectId = table.Column<Guid>(nullable: false),
                    WorkerUserId = table.Column<Guid>(nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectManagersUsers");

            migrationBuilder.DropTable(
                name: "ProjectWorkersUsers");
        }
    }
}
