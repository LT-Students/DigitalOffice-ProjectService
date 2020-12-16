using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class EditProjectUserByIdCommand : IEditProjectUserByIdCommand
    {
        private readonly IProjectRepository repository;
        private readonly IValidator<EditProjectUserRequest> validator;
        private readonly IAccessValidator accessValidator;

        public EditProjectUserByIdCommand(
            [FromServices] IProjectRepository repository,
            [FromServices] IValidator<EditProjectUserRequest> validator,
            [FromServices] IAccessValidator accessValidator)
        {
            this.repository = repository;
            this.validator = validator;
            this.accessValidator = accessValidator;
        }

        public Guid Execute(EditProjectUserRequest request)
        {
            validator.ValidateAndThrowCustom(request);

            const int accessRightId = 2;

            if (!(accessValidator.IsAdmin() || accessValidator.HasRights(accessRightId)))
            {
                throw new ForbiddenException("Not enough rights");
            }

            var dbProjectUser = repository.GetProjectUserById(request.ProjectUserId);
            request.Patch.ApplyTo(dbProjectUser);

            return repository.EditProjectUserById(dbProjectUser);
        }
    }
}
