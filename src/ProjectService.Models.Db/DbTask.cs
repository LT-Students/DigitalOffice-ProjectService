using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
    public class DbTask
    {
        public const string TableName = "Tasks";

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ProjectId { get; set; }
        public string Description { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid TypeId { get; set; }
        public Guid StatusId { get; set; }
        public Guid PriorityId { get; set; }
        public int? PlannedMinutes { get; set; }
        public Guid? ParentId { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Number { get; set; }

        public DbProject Project { get; set; }
        public DbProjectUser Author { get; set; }
        public DbProjectUser AssignedUser { get; set; }
        public DbTaskProperty Status { get; set; }
        public DbTaskProperty Priority { get; set; }
        public DbTaskProperty Type { get; set; }
        
        public DbTask ParentTask { get; set; }

        public ICollection<DbTask> Subtasks { get; set; }
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
                .Property(t => t.Name)
                .IsRequired();

            builder
                .HasOne(t => t.AssignedUser)
                .WithMany(pu => pu.AssignedUserTasks)
                .HasForeignKey(t => t.AssignedTo);

            builder
                .HasOne(t => t.Author)
                .WithMany(pu => pu.AuthorTasks)
                .HasForeignKey(t => t.AuthorId)
                .HasPrincipalKey(pu => pu.UserId);

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
                .HasOne(t => t.ParentTask)
                .WithMany(t => t.Subtasks)
                .HasForeignKey(t => t.ParentId);

            builder
                .HasMany(t => t.Subtasks)
                .WithOne(tp => tp.ParentTask)
                .HasForeignKey(t => t.ParentId);
        }
    }
}