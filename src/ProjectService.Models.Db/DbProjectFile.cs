using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
    public class DbProjectFile
    {
        public const string TableName = "ProjectsFiles";

        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid FileId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedAtUtc { get; set; }
        public Guid? ModifiedBy { get; set; }

        public DbProject Project { get; set; }
    }

    public class DbProjectFileConfiguration : IEntityTypeConfiguration<DbProjectFile>
    {
        public void Configure(EntityTypeBuilder<DbProjectFile> builder)
        {
            builder
                .ToTable(DbProjectFile.TableName);

            builder
                .HasKey(pf => pf.Id);

            builder
                .HasOne(pf => pf.Project)
                .WithMany(p => p.Files)
                .HasForeignKey(pf => pf.ProjectId);
        }
    }
}