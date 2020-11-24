using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers
{
    public class RoleExpandedResponseMapper : IRoleExpandedResponseMapper
    {
        private readonly IRoleMapper _roleMapper;

        public RoleExpandedResponseMapper(
            IRoleMapper roleMapper)
        {
            _roleMapper = roleMapper;
        }

        public RoleExpandedResponse Map(DbRole dbRole)
        {
            return new RoleExpandedResponse
            {
                Role = _roleMapper.Map(dbRole)
            };
        }
    }
}
