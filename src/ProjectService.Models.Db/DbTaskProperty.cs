using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
    public class DbTaskProperty
    {
        public const string TableName = "TaskProperties";

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? AuthorId { get; set; }
        public int PropertyType { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public DbProject Project { get; set; }
        public ICollection<DbTask> PriorityTasks { get; set; }
        public ICollection<DbTask> TypeTasks { get; set; }
        public ICollection<DbTask> StatusTasks { get; set; }
    }

    public class DbTaskPropertyConfiguration : IEntityTypeConfiguration<DbTaskProperty>
    {
        public void Configure(EntityTypeBuilder<DbTaskProperty> builder)
        {
            builder
                .ToTable(DbTaskProperty.TableName);

            builder
                .HasKey(tp => tp.Id);

            builder
                .HasOne(tp => tp.Project)
                .WithMany(p => p.TaskProperties)
                .HasForeignKey(tp => tp.ProjectId);

            builder
                .Property(tp => tp.Name)
                .IsRequired();

            builder
                .HasMany(tp => tp.PriorityTasks)
                .WithOne(T => T.Priority);

            builder
                .HasMany(tp => tp.TypeTasks)
                .WithOne(T => T.Type);

            builder
               .HasMany(tp => tp.StatusTasks)
               .WithOne(T => T.Status);
        }
    }
}