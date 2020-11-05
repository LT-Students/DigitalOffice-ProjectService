using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class GetUserProjectsCommand : IGetUserProjectsCommand
    {
        private readonly IProjectRepository _repository;
        private readonly IProjectResponseMapper _mapper;

        public GetUserProjectsCommand(
            [FromServices] IProjectRepository repository,
            [FromServices] IProjectResponseMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IEnumerable<ProjectResponse> Execute(Guid userId, bool showNotActive)
        {
            return _repository.GetUserProjects(userId, showNotActive).Select(p => _mapper.Map(p));
        }
    }
}