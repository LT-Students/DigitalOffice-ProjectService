using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.ProjectService.Data.Provider
{
    public interface IDataProvider
    {
        DbSet<DbProject> Projects { get; set; }

        DbSet<DbRole> Roles { get; set; }

        void SaveModelsChanges();
        object MakeEntityDetached(object obj);
        void EnsureDeleted();
        bool IsInMemory();
    }
}