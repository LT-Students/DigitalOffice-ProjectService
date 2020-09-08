using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef
{
    /// <summary>
    /// A class that defines the tables and its properties in the database of ProjectService.
    /// </summary>
    public class ProjectServiceDbContext : DbContext, IDataProvider
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

        public void SaveModelsChanges()
        {
            this.SaveChanges();
        }
    }
}