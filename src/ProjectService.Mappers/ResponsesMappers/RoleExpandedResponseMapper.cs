using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers
{
    public class RoleExpandedResponseMapper : IRoleExpandedResponseMapper
    {
        private readonly IProjectUserMapper _projectUserMapper;
        private readonly IRoleMapper _roleMapper;

        public RoleExpandedResponseMapper(
            IProjectUserMapper projectUserMapper,
            IRoleMapper roleMapper)
        {
            _projectUserMapper = projectUserMapper;
            _roleMapper = roleMapper;
        }

        public RoleExpandedResponse Map(DbRole dbRole, IEnumerable<DbProjectUser> users)
        {
            return new RoleExpandedResponse
            {
                Role = _roleMapper.Map(dbRole),
                Users = users.Select(async u => await _projectUserMapper.Map(u)).Select(t => t.Result.User).ToList()
            };
        }
    }
}
