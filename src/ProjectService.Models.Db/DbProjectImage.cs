using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
  public class DbProjectImage
  {
    public const string TableName = "ProjectImages";

    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid ImageId { get; set; }

    public DbProject Project { get; set; }
  }

  public class DbProjectImageConfiguration : IEntityTypeConfiguration<DbProjectImage>
  {
    public void Configure(EntityTypeBuilder<DbProjectImage> builder)
    {
      builder
        .ToTable(DbProjectImage.TableName);

      builder
        .HasKey(p => p.Id);

      builder
        .HasOne(pu => pu.Project)
        .WithMany(p => p.Images);
    }
  }
}
