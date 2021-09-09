using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
    public class DbTaskImage
    {
        public const string TableName = "TasksImages";

        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public Guid ImageId { get; set; }

        public DbTask Task { get; set; }
    }

    public class DbTaskImageConfiguration : IEntityTypeConfiguration<DbTaskImage>
    {
        public void Configure(EntityTypeBuilder<DbTaskImage> builder)
        {
            builder
                .ToTable(DbTaskImage.TableName);

            builder
                .HasKey(p => p.Id);

            builder
                .HasOne(pu => pu.Task)
                .WithMany(p => p.TasksImages);
        }
    }
}
