using System;
using System.Linq;
using FluentValidation;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.User;
using LT.DigitalOffice.ProjectService.Validation.User.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation.User
{
  public class EditProjectUsersRoleRequestValidator : AbstractValidator<(Guid projectId, EditProjectUsersRoleRequest request)>, IEditProjectUsersRoleRequestValidator
  {
    public EditProjectUsersRoleRequestValidator(
      IProjectUserRepository projectUserRepository,
      IProjectRepository projectRepository)
    {
      RuleFor(x => x.projectId)
        .NotEmpty()
        .WithMessage("Request must have a project Id")
        .MustAsync(async (projectId, _) => await projectRepository.DoesExistAsync(projectId))
        .WithMessage("This project id does not exist");

      RuleFor(x => x.request.Role)
        .IsInEnum()
        .WithMessage("Wrong project user's role");

      RuleForEach(x => x.request.UsersIds)
        .NotEmpty()
        .WithMessage("Wrong userId value.")
        .DependentRules(() =>
        {
          RuleFor(x => x)
            .MustAsync(async (x, _) =>
              (await projectUserRepository.DoExistAsync(x.projectId, x.request.UsersIds)).Count() == x.request.UsersIds.Count())
            .WithMessage("Some users don't exist.");
        });
    }
  }
}
