using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.BrokerSupport.Attributes.ParseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
  [ParseEntity]
  public class DbProject
  {
    public const string TableName = "Projects";

    public Guid Id { get; set; }
    public int Status { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Description { get; set; }
    public string ShortDescription { get; set; }
    public string Customer { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }

    [IgnoreParse]
    public DbProjectDepartment Department { get; set; }
    [IgnoreParse]
    public ICollection<DbProjectUser> Users { get; set; }
    [IgnoreParse]
    public ICollection<DbProjectFile> Files { get; set; }
    [IgnoreParse]
    public ICollection<DbProjectImage> Images { get; set; }

    public DbProject()
    {
      Department = new DbProjectDepartment();
      Users = new HashSet<DbProjectUser>();
      Files = new HashSet<DbProjectFile>();
      Images = new HashSet<DbProjectImage>();
    }
  }

  public class DbProjectConfiguration : IEntityTypeConfiguration<DbProject>
  {
    public void Configure(EntityTypeBuilder<DbProject> builder)
    {
      builder
        .ToTable(DbProject.TableName);

      builder
        .HasKey(p => p.Id);

      builder
        .Property(P => P.Name)
        .HasMaxLength(150)
        .IsRequired();

      builder
        .Property(p => p.ShortName)
        .HasMaxLength(150)
        .IsRequired();

      builder
        .Property(p => p.ShortDescription)
        .HasMaxLength(300);

      builder
        .Property(p => p.Customer)
        .HasMaxLength(150);

      builder
        .HasOne(p => p.Department)
        .WithOne(u => u.Project);

      builder
        .HasMany(p => p.Users)
        .WithOne(u => u.Project);

      builder
        .HasMany(p => p.Files)
        .WithOne(f => f.Project);

      builder
       .HasMany(p => p.Images)
       .WithOne(tp => tp.Project);
    }
  }
}
