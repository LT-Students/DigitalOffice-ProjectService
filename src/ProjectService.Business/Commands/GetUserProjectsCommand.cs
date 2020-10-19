using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class GetUserProjectsCommand : IGetUserProjectsCommand
    {
        private readonly IProjectRepository repository;
        private readonly IMapper<DbProject, Project> mapper;

        public GetUserProjectsCommand(
            [FromServices] IProjectRepository repository,
            [FromServices] IMapper<DbProject, Project> mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public IEnumerable<Project> Execute(Guid userId)
        {
            return repository.GetUserProjects(userId).Select(p => mapper.Map(p)).ToList();
        }
    }
}