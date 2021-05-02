using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
    public class DbProject
    {
        public const string TableName = "Projects";

        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid AuthorId { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<DbTask> Tasks { get; set; }
        public ICollection<DbTaskProperty> TaskProperties { get; set; }
        public ICollection<DbProjectUser> Users { get; set; }
        public ICollection<DbProjectFile> Files { get; set; }

        public DbProject()
        {
            Users = new HashSet<DbProjectUser>();
            
            Tasks = new HashSet<DbTask>();
            TaskProperties = new HashSet<DbTaskProperty>();
            Files = new HashSet<DbProjectFile>();
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
                .IsRequired();

            builder
                .Property(P => P.Name)
                .HasMaxLength(150);

            builder
                .Property(p => p.ShortName)
                .HasMaxLength(30);

            builder
                .Property(p => p.ShortDescription)
                .HasMaxLength(300);

            builder
                .HasMany(p => p.Users)
                .WithOne(u => u.Project);
        }
    }
}