using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces
{
    [AutoInject]
    public interface IProjectUserMapper
    {
        Task<ProjectUserInfo> Map(DbProjectUser dbProjectUser);
    }
}
