using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.EFSupport.Provider;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.ProjectService.Data.Provider
{
    [AutoInject(InjectType.Scoped)]
    public interface IDataProvider : IBaseDataProvider
    {
        DbSet<DbProject> Projects { get; set; }
        DbSet<DbProjectFile> ProjectsFiles { get; set; }
        DbSet<DbProjectUser> ProjectsUsers { get; set; }
        DbSet<DbProjectImage> Images { get; set; }
    }
}
