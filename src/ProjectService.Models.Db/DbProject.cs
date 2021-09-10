using LT.DigitalOffice.Kernel.Attributes.ParseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
    [ParseEntity]
    public class DbProject
    {
        public const string TableName = "Projects";

        public Guid Id { get; set; }
        public Guid? DepartmentId { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedAtUtc { get; set; }
        public Guid? ModifiedBy { get; set; }

        [IgnoreParse]
        public ICollection<DbTask> Tasks { get; set; }
        [IgnoreParse]
        public ICollection<DbProjectUser> Users { get; set; }
        [IgnoreParse]
        public ICollection<DbProjectFile> Files { get; set; }
        [IgnoreParse]
        public ICollection<DbTaskProperty> TaskProperties { get; set; }
        [IgnoreParse]
        public ICollection<DbProjectImage> ProjectsImages { get; set; }

        public DbProject()
        {
            Tasks = new HashSet<DbTask>();

            Users = new HashSet<DbProjectUser>();

            Files = new HashSet<DbProjectFile>();

            TaskProperties = new HashSet<DbTaskProperty>();

            ProjectsImages = new HashSet<DbProjectImage>();
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
                .IsRequired();

            builder
                .Property(p => p.ShortName)
                .HasMaxLength(30);

            builder
                .Property(p => p.ShortDescription)
                .HasMaxLength(300);

            builder
                .HasMany(p => p.Users)
                .WithOne(u => u.Project);

            builder
                .HasMany(p => p.Files)
                .WithOne(f => f.Project);

            builder
                .HasMany(p => p.Tasks)
                .WithOne(t => t.Project);

            builder
                .HasMany(p => p.TaskProperties)
                .WithOne(tp => tp.Project);

            builder
               .HasMany(p => p.ProjectsImages)
               .WithOne(tp => tp.Project);
        }
    }
}