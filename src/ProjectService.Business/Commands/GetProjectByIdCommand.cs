using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class GetProjectByIdCommand : IGetProjectInfoByIdCommand
    {
        private readonly IProjectRepository repository;
        private readonly IMapper<DbProject, Project> mapper;

        public GetProjectByIdCommand(
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