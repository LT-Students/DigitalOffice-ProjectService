using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponseModels;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelMappers.Interfaces
{
    public interface IRoleMapper : IMapper<DbRole, Role> { }
}
