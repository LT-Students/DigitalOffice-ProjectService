using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Db
{
    public class DbRole
    {
        public const string TableName = "Roles";

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public ICollection<DbProjectUser> Users { get; set; }
    }

    public class DbRoleConfiguration : IEntityTypeConfiguration<DbRole>
    {
        public void Configure(EntityTypeBuilder<DbRole> builder)
        {
            builder
                .ToTable(DbRole.TableName);

            builder
                .HasKey(r => r.Id);

            builder
                .HasMany(r => r.Users)
                .WithOne(u => u.Role);
        }
    }
}
