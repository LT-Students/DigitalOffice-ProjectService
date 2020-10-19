using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;


namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class GetUserProjectsCommand : IGetUserProjectsCommand
    {
        private IProjectRepository repository;
        public GetUserProjectsCommand([FromServices] IProjectRepository repository)
        {
            this.repository = repository;
        }
        public List<DbProject> Execute(Guid userId)
        {
            return repository.GetUserProjects(userId);
        }
    }
}