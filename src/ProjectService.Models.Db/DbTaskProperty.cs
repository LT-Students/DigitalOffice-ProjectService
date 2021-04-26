using System;
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
                .Property(t => t.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder
                .Property(t => t.Type)
                .IsRequired();
        }
    }
}