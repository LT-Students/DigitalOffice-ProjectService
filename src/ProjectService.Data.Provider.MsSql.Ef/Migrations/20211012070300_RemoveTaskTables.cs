using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(ProjectServiceDbContext))]
  [Migration("20211012070300_RemoveTaskTables")]
  public class RemoveTaskTables : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable("Tasks");
      migrationBuilder.DropTable("TaskProperties");
    }
  }
}
