using FluentValidation;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands
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

            validator.ValidateAndThrowCustom(request);
            var dbProject = mapper.Map(request);

            return repository.EditProjectById(dbProject);
        }
    }
}