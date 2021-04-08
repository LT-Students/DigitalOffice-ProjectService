using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces
{
    [AutoInject]
    public interface IProjectResponseMapper : IMapper<DbProject, Project> { }
}
