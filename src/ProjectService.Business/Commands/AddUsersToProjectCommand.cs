using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class AddUsersToProjectCommand : IAddUsersToProjectCommand
    {
        const int ACCESS_RIGHT_ID = 2;

        private readonly IUserRepository _repository;
        private readonly IProjectUserRequestMapper _mapper;
        private readonly IAccessValidator _accessValidator;
        private readonly IAddUsersToProjectValidator _validator;

        public AddUsersToProjectCommand(
            IUserRepository repository,
            IProjectUserRequestMapper mapper,
            IAccessValidator accessValidator,
            IAddUsersToProjectValidator validator)
        {
            _mapper = mapper;
            _validator = validator;
            _repository = repository;
            _accessValidator = accessValidator;
        }

        public void Execute(AddUsersToProjectRequest request)
        {
            if (!(_accessValidator.IsAdmin() || _accessValidator.HasRights(ACCESS_RIGHT_ID)))
            {
                throw new ForbiddenException("Not enough rights");
            }

            _validator.ValidateAndThrowCustom(request);

            List<DbProjectUser> dbProjectUser = request.Users.Select(user =>
                GetDbProjectUsers(user, request.ProjectId)
            ).ToList();

            _repository.AddUsersToProject(dbProjectUser, request.ProjectId);
        }

        public DbProjectUser GetDbProjectUsers(ProjectUserRequest projectUser, Guid projectId)
        {
            DbProjectUser dbProjectUser = _mapper.Map(projectUser);

            dbProjectUser.Id = projectId;

            return dbProjectUser;
        }
    }
}
