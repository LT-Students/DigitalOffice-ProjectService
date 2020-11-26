using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponseModels;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelMappers.Interfaces
{
    public interface IProjectUserMapper
    {
        Task<ProjectUser> Map(DbProjectUser dbProjectUser);
    }
}
