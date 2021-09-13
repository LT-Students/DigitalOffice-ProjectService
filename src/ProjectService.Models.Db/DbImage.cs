using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
    public class DbImage
    {
        public const string TableName = "Images";

        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public Guid ImageId { get; set; }

        public DbProject Project { get; set; }
        public DbTask Task { get; set; }
    }

    public class DbProjectImageConfiguration : IEntityTypeConfiguration<DbImage>
    {
        public void Configure(EntityTypeBuilder<DbImage> builder)
        {
            builder
                .ToTable(DbImage.TableName);

            builder
                .HasKey(p => p.Id);

            builder
                .HasOne(pu => pu.Project)
                .WithMany(p => p.Images);

            builder
                .HasOne(pu => pu.Project)
                .WithMany(p => p.Images);
        }
    }
}
