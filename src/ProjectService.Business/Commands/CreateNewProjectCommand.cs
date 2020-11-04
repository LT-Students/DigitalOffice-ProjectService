using FluentValidation;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class CreateNewProjectCommand : ICreateNewProjectCommand
    {
        private readonly IProjectRepository repository;
        private readonly IValidator<NewProjectRequest> validator;
        private readonly IProjectMapper mapper;

        public CreateNewProjectCommand(
            [FromServices] IProjectRepository repository,
            [FromServices] IValidator<NewProjectRequest> validator,
            [FromServices] IProjectMapper mapper)
        {
            this.repository = repository;
            this.validator = validator;
            this.mapper = mapper;
        }

        public Guid Execute(NewProjectRequest request)
        {
            validator.ValidateAndThrowCustom(request);

            var dbProject = mapper.Map(request);

            return repository.CreateNewProject(dbProject);
        }
    }
}
