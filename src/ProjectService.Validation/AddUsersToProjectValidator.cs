using FluentValidation;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class AddUsersToProjectValidator : AbstractValidator<AddUsersToProjectRequest>, IAddUsersToProjectValidator
    {
        private readonly IProjectRepository _repository;

        private IEnumerable<DbProjectUser> _dbPrjectUsers;

        public AddUsersToProjectValidator(IProjectRepository repository)
        {
            _repository = repository;

            RuleFor(projectUser => projectUser.ProjectId)
               .NotEmpty()
               .WithMessage("Request must have a project Id")
               .Must(projectId => CheckTheProjecForExistenceInDb(projectId))
               .WithMessage("This project id does not exist")
               .DependentRules(() =>
               {
                   RuleFor(projectUser => projectUser.Users)
                       .Must(user => user != null && user.Count() != 0)
                       .WithMessage("The request must contain users");

                   RuleForEach(projectUser => projectUser.Users)
                       .SetValidator(new ProjectUserValidator())
                       .Must(user => CheckTheUserForExistenceInDb(user))
                       .WithMessage("This user is already exist - list index: {CollectionIndex}");
               });
        }

        private bool CheckTheProjecForExistenceInDb(Guid projectId)
        {
            bool showNotActiveUsers = false;

            _dbPrjectUsers = _repository.GetProjectUsers(projectId, showNotActiveUsers);

            return _dbPrjectUsers != null;
        }

        private bool CheckTheUserForExistenceInDb(ProjectUser user)
        {
            foreach (var dbUser in _dbPrjectUsers)
            {
                if (user.Id == dbUser.Id)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
