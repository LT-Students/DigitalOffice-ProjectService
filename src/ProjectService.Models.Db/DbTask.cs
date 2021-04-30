using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
    public class DbTask
    {
        public const string TableName = "Tasks";

        public string Name { get; set; }
        public string Description { get; set; }
        public Guid ProjectId { get; set; }
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid PriorityId { get; set; }
        public Guid StatusId { get; set; }
        public Guid TypeId { get; set; }
        public DateTime CreateTime { get; set; }
        public int? PlannedMinutes { get; set; }
        public int Number { get; set; }
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
                .HasMaxLength(150)
                .IsRequired();

            builder
                .Property(t => t.AuthorId)
                .IsRequired();

            builder
                .Property(t => t.ProjectId)
                .IsRequired();

            builder
                .Property(t => t.PriorityId)
                .IsRequired();

            builder
                .Property(t => t.StatusId)
                .IsRequired();

            builder
                .Property(t => t.TypeId)
                .IsRequired();

            builder
                .Property(t => t.CreateTime)
                .IsRequired();

            builder
                .Property(t => t.Number)
                .IsRequired();
        }
    }
}
