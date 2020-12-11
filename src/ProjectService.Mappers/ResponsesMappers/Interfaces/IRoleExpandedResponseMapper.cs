using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces
{
    public interface IRoleExpandedResponseMapper : IMapper<DbRole, RoleExpandedResponse>
    {
        RoleExpandedResponse Map(DbRole dbRole);
    }
}
