﻿using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class GetRoleCommand : IGetRoleCommand
    {
        private readonly IRoleRepository _repository;
        private readonly IRoleExpandedResponseMapper _mapper;

        public GetRoleCommand(
            [FromServices] IRoleRepository repository,
            [FromServices] IRoleExpandedResponseMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public RoleExpandedResponse Execute(Guid roleId, bool showNotActiveUsers)
        {
            var dbRole = _repository.GetRole(roleId);

            var dbProjectUsers = _repository.GetProjectUsers(roleId);

            return _mapper.Map(dbRole, dbProjectUsers);
        }
    }
}