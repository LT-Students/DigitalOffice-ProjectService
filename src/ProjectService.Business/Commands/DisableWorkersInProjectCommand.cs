﻿using FluentValidation;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class DisableWorkersInProjectCommand : IDisableWorkersInProjectCommand
    {
        private readonly IProjectRepository repository;
        private readonly IValidator<WorkersIdsInProjectRequest> validator;

        public DisableWorkersInProjectCommand(
            [FromServices] IProjectRepository repository,
            [FromServices] IValidator<WorkersIdsInProjectRequest> validator)
        {
            this.repository = repository;
            this.validator = validator;
        }

        public void Execute(WorkersIdsInProjectRequest request)
        {
            validator.ValidateAndThrowCustom(request);

            repository.DisableWorkersInProject(request);
        }
    }
}
