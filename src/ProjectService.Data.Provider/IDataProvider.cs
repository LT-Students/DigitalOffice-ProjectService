using Microsoft.EntityFrameworkCore;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;

namespace LT.DigitalOffice.ProjectService.Data.Provider
{
    public interface IDataProvider
    {
        DbSet<DbProject> Projects { get; set; }

        void SaveModelsChanges();
    }
}