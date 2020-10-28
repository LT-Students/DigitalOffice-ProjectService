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
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public Guid DepartmentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int? ClosedReason { get; set; }
        public bool IsActive { get; set; }

        public ICollection<DbProjectUser> Users { get; set; }
        public ICollection<DbProjectFile> Files { get; set; }

        public DbProject()
        {
            Users = new HashSet<DbProjectUser>();

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
                .HasMany(p => p.Users)
                .WithOne(u => u.Project);

            builder
                .Property(p => p.Name)
                .IsRequired();
        }
    }
}