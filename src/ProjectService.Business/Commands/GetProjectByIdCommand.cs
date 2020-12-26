using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class GetProjectByIdCommand : IGetProjectByIdCommand
    {
        private readonly IProjectRepository _repository;
        private readonly IProjectExpandedResponseMapper _mapper;

        public GetProjectByIdCommand(
            [FromServices] IProjectRepository repository,
            [FromServices] IProjectExpandedResponseMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProjectExpandedResponse> Execute(Guid projectId, bool showNotActiveUsers)
        {
            var dbProject = _repository.GetProject(projectId);

            var dbProjectUsers = _repository.GetProjectUsers(projectId, showNotActiveUsers);

            return await _mapper.Map(dbProject, dbProjectUsers);
        }
    }
}