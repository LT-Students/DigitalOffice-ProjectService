﻿using System.Reflection;
using System.Threading.Tasks;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef
{
  /// <summary>
  /// A class that defines the tables and its properties in the database of ProjectService.
  /// </summary>
  public class ProjectServiceDbContext : DbContext, IDataProvider
  {
    public DbSet<DbProject> Projects { get; set; }
    public DbSet<DbProjectDepartment> ProjectsDepartments { get; set; }
    public DbSet<DbProjectFile> ProjectsFiles { get; set; }
    public DbSet<DbProjectUser> ProjectsUsers { get; set; }
    public DbSet<DbProjectImage> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(
        Assembly.Load("LT.DigitalOffice.ProjectService.Models.Db"));
    }

    public ProjectServiceDbContext(DbContextOptions<ProjectServiceDbContext> options)
        : base(options)
    {
    }

    public void Save()
    {
      SaveChanges();
    }

    public object MakeEntityDetached(object obj)
    {
      Entry(obj).State = EntityState.Detached;

      return Entry(obj).State;
    }

    public void EnsureDeleted()
    {
      Database.EnsureDeleted();
    }

    public bool IsInMemory()
    {
      return Database.IsInMemory();
    }

    public async Task SaveAsync()
    {
      await SaveChangesAsync();
    }
  }
}
