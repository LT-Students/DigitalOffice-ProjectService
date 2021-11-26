using System.Linq;
using FluentValidation;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.User.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation.User
{
  public class AddUsersToProjectValidator : AbstractValidator<CreateProjectUsersRequest>, IAddUsersToProjectValidator
  {
    public AddUsersToProjectValidator(
      IProjectUserValidator projectUserValidator,
      IProjectRepository projectRepository)
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
            .Must(user => user != null && user.Any())
            .WithMessage("The request must contain users");

          RuleForEach(projectUser => projectUser.Users)
            .SetValidator(projectUserValidator);
        });
    }
  }
}
