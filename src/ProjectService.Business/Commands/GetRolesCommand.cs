using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class GetRolesCommand : IGetRolesCommand
    {
        private readonly IRoleRepository _repository;
        private readonly IRolesResponseMapper _mapper;

        public GetRolesCommand(
            [FromServices] IRoleRepository repository,
            [FromServices] IRolesResponseMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public RolesResponse Execute(int skip, int take)
        {
            var dbRoles = _repository.GetRoles(skip, take);

            return _mapper.Map(dbRoles);
        }
    }
}
