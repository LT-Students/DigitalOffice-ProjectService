using LT.DigitalOffice.ProjectService.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LT.DigitalOffice.ProjectService.Database
{
    /// <summary>
    /// A class that defines the tables and its properties in the database of ProjectService.
    /// </summary>
    public class ProjectServiceDbContext : DbContext
    {
        public ProjectServiceDbContext (DbContextOptions<ProjectServiceDbContext> options)
            :base(options)
        {
        }

        public DbSet<DbProject> Projects { get; set; }

        // Fluent API is written here.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}