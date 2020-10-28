using LT.DigitalOffice.Kernel.Database;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.ProjectService.Data.Provider
{
    public interface IDataProvider : IBaseDataProvider
    {
        DbSet<DbProject> Projects { get; set; }
        DbSet<DbProjectFile> ProjectsFiles { get; set; }
        DbSet<DbProjectUser> ProjectsUsers { get; set; }
        DbSet<DbRole> Roles { get; set; }
    }
}