using LT.DigitalOffice.ProjectService.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Database.Entities;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models;
using LT.DigitalOffice.ProjectService.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.ProjectService.Commands
{
    public class GetProjectInfoByIdCommand : IGetProjectInfoByIdCommand
    {
        private readonly IProjectRepository repository;
        private readonly IMapper<DbProject, Project> mapper;

        public GetProjectInfoByIdCommand(
            [FromServices] IProjectRepository repository,
            [FromServices] IMapper<DbProject, Project> mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public Project Execute(Guid projectId)
        {
            return mapper.Map(repository.GetProjectInfoById(projectId));
        }
    }
}