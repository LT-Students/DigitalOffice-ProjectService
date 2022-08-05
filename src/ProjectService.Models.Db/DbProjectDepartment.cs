using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
  public class DbProjectDepartment
  {
    public const string TableName = "ProjectsDepartments";
    public const string HistoryTableName = "ProjectsDepartmentsHistory";

    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid DepartmentId { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedBy { get; set; }

    public DbProject Project { get; set; }
  }

  public class DbProjectDepartmentConfiguration : IEntityTypeConfiguration<DbProjectDepartment>
  {
    public void Configure(EntityTypeBuilder<DbProjectDepartment> builder)
    {
      builder
        .ToTable(DbProjectDepartment.TableName, pd => pd.IsTemporal(h =>
        {
          h.UseHistoryTable(DbProjectDepartment.HistoryTableName);
        }));

      builder
        .HasKey(pd => pd.Id);
    }
  }
}
