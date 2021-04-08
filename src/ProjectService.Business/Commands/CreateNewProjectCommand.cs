using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class CreateNewProjectCommand : ICreateNewProjectCommand
    {
        private readonly IProjectRepository repository;
        private readonly IValidator<ProjectExpandedRequest> validator;
        private readonly IProjectExpandedRequestMapper mapper;
        private readonly IAccessValidator accessValidator;

        public CreateNewProjectCommand(
            IProjectRepository repository,
            IValidator<ProjectExpandedRequest> validator,
            IProjectExpandedRequestMapper mapper,
            IAccessValidator accessValidator)
        {
            this.repository = repository;
            this.validator = validator;
            this.mapper = mapper;
            this.accessValidator = accessValidator;
        }

        public Guid Execute(ProjectExpandedRequest request)
        {
            const int accessRightId = 2;

            if (!(accessValidator.IsAdmin() || accessValidator.HasRights(accessRightId)))
            {
                throw new ForbiddenException("Not enough rights");
            }

            validator.ValidateAndThrowCustom(request);

            var dbProject = mapper.Map(request);

            return repository.CreateNewProject(dbProject);
        }
    }
}
