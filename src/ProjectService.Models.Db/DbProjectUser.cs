using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
    public class DbProjectUser
    {
        public const string TableName = "ProjectsUsers";

        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public int Role { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime? RemovedOn { get; set; }
        public bool IsActive { get; set; }

        public DbProject Project { get; set; }

        public ICollection<DbTask> Tasks { get; set; }
        public ICollection<DbTaskProperty> TaskProperties { get; set; }

        public DbProjectUser ()
        {
            Tasks = new HashSet<DbTask>();

            TaskProperties = new HashSet<DbTaskProperty>();
        }
    }

    public class DbProjectUserConfiguration : IEntityTypeConfiguration<DbProjectUser>
    {
        public void Configure(EntityTypeBuilder<DbProjectUser> builder)
        {
            builder
                .ToTable(DbProjectUser.TableName);

            builder
                .HasKey(pu => pu.Id);

            builder
                .HasOne(pu => pu.Project)
                .WithMany(p => p.Users)
                .HasForeignKey(pu => pu.ProjectId);
        }
    }
}