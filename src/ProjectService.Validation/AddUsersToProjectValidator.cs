using FluentValidation;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class AddUsersToProjectValidator : AbstractValidator<AddUsersToProjectRequest>, IAddUsersToProjectValidator
    {
        private readonly IUserRepository _userRepository;
        private List<Guid> _existingUsers = new();

        public AddUsersToProjectValidator(
            IProjectRepository projectRepository,
            IUserRepository userRepository
        )
        {
            CascadeMode = CascadeMode.Stop;
            
            _userRepository = userRepository;

            RuleFor(projectUser => projectUser.ProjectId)
                .NotEmpty()
                .WithMessage("Request must have a project Id")
                .Must(projectRepository.IsExist)
                .WithMessage("This project id does not exist")
                .DependentRules(() =>
                {
                    RuleFor(projectUser => projectUser.Users)
                        .Must(user => user != null && user.Any())
                        .WithMessage("The request must contain users");

                    RuleForEach(projectUser => projectUser.Users)
                        .SetValidator(new ProjectUserValidator());
                });
            
            RuleFor(pu => pu)
                .NotEmpty()
                .Must(pu => !AreUsersInProject(pu.Users, pu.ProjectId, out _existingUsers))
                .WithMessage($"Users with the following ids are already exist {string.Join(", ", _existingUsers)}");
        }

        private bool AreUsersInProject(
            IEnumerable<ProjectUserRequest> users,
            Guid projectId,
            out List<Guid> existingUsers)
        {
            existingUsers = users
                .Select(u => u.UserId)
                .Intersect(_userRepository
                    .GetProjectUsers(projectId, false)
                    .Where(p => p.ProjectId == projectId)
                    .Select(p => p.Id)
                ).ToList();

            return existingUsers.Any();
        }
    }
}