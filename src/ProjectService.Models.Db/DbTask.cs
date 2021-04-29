using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
    public class DbTask
    {
        public const string TableName = "Tasks";

        public Guid Id { get; set; }
        public Guid TypeId { get; set; }
        public Guid AuthorId { get; set; }
        public Guid StatusId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid PriorityId { get; set; }
        public Guid? AssignedTo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
        public int? PlannedMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? Deadline { get; set; }

        public DbProject Project { get; set; }
        public DbProjectUser User { get; set; }
        public DbTaskProperty Status { get; set; }
        public DbTaskProperty Priority { get; set; }
        public DbTaskProperty Type { get; set; }
    }

    public class DbTaskConfiguration : IEntityTypeConfiguration<DbTask>
    {
        public void Configure(EntityTypeBuilder<DbTask> builder)
        {
            builder
                .ToTable(DbTask.TableName);

            builder
                .HasKey(t => t.Id);

            builder
                .HasOne(t => t.User)
                .WithMany(pu => pu.Tasks)
                .HasForeignKey(t => t.AuthorId);

            builder
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId);

            builder
                .HasOne(t => t.Status)
                .WithMany(tp => tp.StatusTasks)
                .HasForeignKey(t => t.StatusId);

            builder
                .HasOne(t => t.Type)
                .WithMany(tp => tp.TypeTasks)
                .HasForeignKey(t => t.TypeId);

            builder
                .HasOne(t => t.Priority)
                .WithMany(tp => tp.PriorityTasks)
                .HasForeignKey(t => t.PriorityId);

            builder
                .Property(t => t.Name)
                .IsRequired();
        }
    }
}