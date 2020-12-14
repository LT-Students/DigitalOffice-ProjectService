using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces
{
    public interface IProjectUserRequestMapper : IMapper<ProjectUserRequest, DbProjectUser>
    {
    }
}
