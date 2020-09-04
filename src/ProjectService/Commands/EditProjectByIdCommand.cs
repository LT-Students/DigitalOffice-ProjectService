using FluentValidation;
using LT.DigitalOffice.ProjectService.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Database.Entities;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models;
using LT.DigitalOffice.ProjectService.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.ProjectService.Commands
{
    public class EditProjectByIdCommand : IEditProjectByIdCommand
    {
        private readonly IMapper<EditProjectRequest, DbProject> mapper;
        private readonly IValidator<EditProjectRequest> validator;
        private readonly IProjectRepository repository;

        public EditProjectByIdCommand(
            [FromServices] IMapper<EditProjectRequest, DbProject> mapper,
            [FromServices] IValidator<EditProjectRequest> validator,
            [FromServices] IProjectRepository repository)
        {
            this.mapper = mapper;
            this.validator = validator;
            this.repository = repository;
        }

        /// <summary>
        /// Contains null request check. If request is null, throws ArgumentNullException. This prevents NullReferenceException
        /// throwing when assigning Guid to null request instance.
        /// </summary>
        public Guid Execute(Guid projectId, EditProjectRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }

            request.Id = projectId;

            validator.ValidateAndThrow(request);
            var dbProject = mapper.Map(request);

            return repository.EditProjectById(dbProject);
        }
    }
}