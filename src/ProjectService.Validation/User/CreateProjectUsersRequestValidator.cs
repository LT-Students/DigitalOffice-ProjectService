using System;
using System.Linq;
using FluentValidation;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.User;
using LT.DigitalOffice.ProjectService.Validation.User.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation.User
{
  public class CreateProjectUsersRequestValidator : AbstractValidator<CreateProjectUsersRequest>, ICreateProjectUsersRequestValidator
  {
    public CreateProjectUsersRequestValidator(
      IProjectRepository projectRepository,
      IProjectUserRepository projectUserRepository,
      IUserService userService)
    {
      CascadeMode = CascadeMode.Stop;

      RuleFor(request => request.ProjectId)
        .NotEmpty()
        .WithMessage("Request must have a project Id")
        .MustAsync(async (projectId, _) => await projectRepository.DoesExistAsync(projectId))
        .WithMessage("This project id does not exist");

      RuleForEach(x => x.Users)
        .Must(x => x.UserId != default && Enum.IsDefined(typeof(ProjectUserRoleType), x.Role))
        .WithMessage("Wrong userId value.")
        .DependentRules(() =>
        {
          RuleFor(x => x)
            .MustAsync(async (r, _) =>
              (await userService.CheckUsersExistenceAsync(r.Users.Select(x => x.UserId).ToList())).Count() == r.Users.Count)
            .WithMessage("Some users does not exist.")
            .MustAsync(async (r, _) =>
              !(await projectUserRepository.DoExistAsync(r.ProjectId, r.Users.Select(x => x.UserId), isActive: true)).Any())
            .WithMessage("Some users are already in this project.");
        });
    }
  }
}
