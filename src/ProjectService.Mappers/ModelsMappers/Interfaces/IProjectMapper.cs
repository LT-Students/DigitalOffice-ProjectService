using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces
{
    public interface IProjectMapper : IMapper<DbProject, Project> { }
}
