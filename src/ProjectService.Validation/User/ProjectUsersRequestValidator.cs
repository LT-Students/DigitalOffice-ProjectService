using System.Linq;
using FluentValidation;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.User.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation.User
{
  public class ProjectUsersRequestValidator : AbstractValidator<ProjectUsersRequest>, IProjectUsersRequestValidator
  {
    public ProjectUsersRequestValidator(
      IUserRequestValidator projectUserValidator,
      IProjectRepository projectRepository,
      IUserService userService)
    {
      CascadeMode = CascadeMode.Stop;

      RuleFor(projectUser => projectUser.ProjectId)
        .NotEmpty()
        .WithMessage("Request must have a project Id")
        .MustAsync(async (x, _) => await projectRepository.DoesExistAsync(x))
        .WithMessage("This project id does not exist")
        .DependentRules(() =>
        {
          RuleFor(projectUser => projectUser.Users)
            .Must(users => users is not null && users.Any())
            .WithMessage("The request must contain users")
            .MustAsync(async (projectUser, cancellation) =>
              (await userService.CheckUsersExistenceAsync(projectUser.Select(user => user.UserId).ToList())).Count() == projectUser.Count())
            .WithMessage("Some users does not exist.");

          RuleForEach(projectUser => projectUser.Users)
            .SetValidator(projectUserValidator);
        });
    }
  }
}
