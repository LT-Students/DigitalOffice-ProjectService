using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponseModels;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponseMappers.Interfaces
{
    public interface IProjectResponseMapper : IMapper<DbProject, Project> { }
}
