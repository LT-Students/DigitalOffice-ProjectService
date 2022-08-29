using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Text.Json.Serialization;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
  public class DbProjectUser
  {
    public const string TableName = "ProjectsUsers";
    public const string HistoryTableName = "ProjectsUsersHistory";

    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public int Role { get; set; }
    public Guid CreatedBy { get; set; }
    public bool IsActive { get; set; }

    [JsonIgnore]
    public DbProject Project { get; set; }
  }

  public class DbProjectUserConfiguration : IEntityTypeConfiguration<DbProjectUser>
  {
    public void Configure(EntityTypeBuilder<DbProjectUser> builder)
    {
      builder
        .ToTable(
          DbProjectUser.TableName,
          pu => pu.IsTemporal(
            builder => builder.UseHistoryTable(DbProjectUser.HistoryTableName)));

      builder
        .HasKey(pu => pu.Id);

      builder
        .HasOne(pu => pu.Project)
        .WithMany(p => p.Users)
        .HasForeignKey(pu => pu.ProjectId);
    }
  }
}
