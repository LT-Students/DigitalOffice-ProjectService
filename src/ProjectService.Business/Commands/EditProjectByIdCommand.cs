﻿using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class EditProjectByIdCommand : IEditProjectByIdCommand
    {
        private readonly IEditProjectValidator validator;
        private readonly IProjectRepository repository;
        private readonly IAccessValidator accessValidator;

        public EditProjectByIdCommand(
            IEditProjectValidator validator,
            IProjectRepository repository,
            IAccessValidator accessValidator)
        {
            this.validator = validator;
            this.repository = repository;
            this.accessValidator = accessValidator;
        }

        public Guid Execute(EditProjectRequest request)
        {
            validator.ValidateAndThrowCustom(request);

            if (!(accessValidator.IsAdmin() || accessValidator.HasRights(Rights.AddEditRemoveProjects)))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            var projectFilter = new GetProjectFilter
            {
                ProjectId = request.ProjectId
            };

            var dbProject = repository.GetProject(projectFilter);

            request.Patch.ApplyTo(dbProject);

            return repository.EditProjectById(dbProject);
        }
    }
}