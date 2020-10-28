using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
    public class DbProjectFile
    {
        public const string TableName = "ProjectsFiles";

        public Guid ProjectId { get; set; }
        public Guid FileId { get; set; }

        public DbProject Project { get; set; }
    }

    public class DbProjectFileConfiguration : IEntityTypeConfiguration<DbProjectFile>
    {
        public void Configure(EntityTypeBuilder<DbProjectFile> builder)
        {
            builder
                .ToTable(DbProjectFile.TableName);

            builder
                .HasKey(projectFile => new { projectFile.ProjectId, projectFile.FileId });

            builder
                .HasOne(pf => pf.Project)
                .WithMany(p => p.Files)
                .HasForeignKey(pf => pf.ProjectId);
        }
    }
}