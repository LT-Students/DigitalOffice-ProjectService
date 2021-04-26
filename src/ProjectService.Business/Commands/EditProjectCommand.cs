using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class EditProjectCommand : IEditProjectCommand
    {
        private readonly IEditProjectValidator _validator;
        private readonly IAccessValidator _accessValidator;
        private readonly IEditProjectMapper _mapper;
        private readonly IProjectRepository _repository;

        public EditProjectCommand(
            IEditProjectValidator validator,
            IAccessValidator accessValidator,
            IEditProjectMapper mapper,
            IProjectRepository repository
          )
        {
            _validator = validator;
            _accessValidator = accessValidator;
            _mapper = mapper;
            _repository = repository;
        }

        public bool Execute(Guid projectId, JsonPatchDocument<EditProjectRequest> request)
        {
            _validator.ValidateAndThrowCustom(request);

            if (!_accessValidator.IsAdmin() && !_accessValidator.HasRights(Kernel.Constants.Rights.AddEditRemoveProjects))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            JsonPatchDocument<DbProject> dbRequest = _mapper.Map(request);

            return _repository.EditProject(projectId, dbRequest);
        }
    }
}