using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
    public class DbProjectImage
    {
        public const string TableName = "ProjectsImages";
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
                .Property(P => P.ImageId)
                .IsRequired();

            builder
                .HasOne(pu => pu.Project)
                .WithMany(p => p.ProjectImages)
                .HasForeignKey(pu => pu.ProjectId);
        }
    }
}
