using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System;

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

        public RoleExpandedResponse Execute(Guid roleId)
        {
            var dbRole = _repository.GetRole(roleId);

            return _mapper.Map(dbRole);
        }
    }
}
