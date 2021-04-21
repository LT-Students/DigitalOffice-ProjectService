using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces
{
    [AutoInject]
    public interface IProjectUserRequestMapper : IMapper<ProjectUser, DbProjectUser>
    {
    }
}
