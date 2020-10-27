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

        public DbSet<DbRole> Roles { get; set; }

        // Fluent API is written here.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                Assembly.Load("LT.DigitalOffice.ProjectService.Models.Db"));
        }

        public void SaveModelsChanges()
        {
            this.SaveChanges();
        }

        public object MakeEntityDetached(object obj)
        {
            this.Entry(obj).State = EntityState.Detached;

            return this.Entry(obj).State;
        }

        public void EnsureDeleted()
        {
            this.Database.EnsureDeleted();
        }

        public bool IsInMemory()
        {
            return this.Database.IsInMemory();
        }
    }
}