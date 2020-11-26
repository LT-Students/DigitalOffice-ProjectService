using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestModels;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestMappers.Interfaces
{
    public interface IProjectUserRequestMapper : IMapper<ProjectUserRequest, DbProjectUser>
    {
    }
}
