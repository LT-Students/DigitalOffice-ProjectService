using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers
{
    public class RolesResponseMapper : IRolesResponseMapper
    {
        private readonly IRoleMapper _roleMapper;

        public RolesResponseMapper(
            IRoleMapper roleMapper)
        {
            _roleMapper = roleMapper;
        }

        public RolesResponse Map(IEnumerable<DbRole> dbRoles)
        {
            return new RolesResponse
            {
                Roles = dbRoles.Select(r => _roleMapper.Map(r)).ToList(),
                TotalCount = dbRoles.Count()
            };
        }
    }
}
