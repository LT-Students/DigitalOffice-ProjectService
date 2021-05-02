using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
    public class DbTaskProperty
    {
        public const string TableName = "TaskProperties";

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public Guid ProjectId { get; set; }
        public Guid AuthorId { get; set; }
        public DbProject Project { get; set; }
        public DbProjectUser User { get; set; }
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
                .HasKey(t => t.Id);

            builder
                .HasOne(tp => tp.User)
                .WithMany(u => u.TaskProperties)
                .HasForeignKey(tp => tp.AuthorId);

            builder
                .HasOne(tp => tp.Project)
                .WithMany(p => p.TaskProperties)
                .HasForeignKey(tp => tp.ProjectId);
            
            builder
                .Property(t => t.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder
                .Property(t => t.Type)
                .IsRequired();
        }
    }
}
