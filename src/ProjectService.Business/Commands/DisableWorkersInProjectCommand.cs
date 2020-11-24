using FluentValidation;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class DisableWorkersInProjectCommand : IDisableWorkersInProjectCommand
    {
        private readonly IProjectRepository repository;
        private readonly IValidator<ProjectRequest> validator;

        public DisableWorkersInProjectCommand(
            [FromServices] IProjectRepository repository,
            [FromServices] IValidator<ProjectRequest> validator)
        {
            this.repository = repository;
            this.validator = validator;
        }

        public void Execute(ProjectRequest request)
        {
            validator.ValidateAndThrow(request);

            repository.DisableWorkersInProject(request);
        }
    }
}
