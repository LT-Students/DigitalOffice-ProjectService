using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces
{
    public interface IProjectUserMapper
    {
        Task<ProjectUser> Map(DbProjectUser dbProjectUser);
    }
}
