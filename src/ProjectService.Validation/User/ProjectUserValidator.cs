using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.User.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation.User
{
  public class ProjectUserValidator : AbstractValidator<AddUserRequest>, IProjectUserValidator
  {
    public ProjectUserValidator()
    {
      RuleFor(pu => pu.UserId)
        .NotEmpty()
        .WithMessage("Not specified user id.");

      RuleFor(pu => pu.Role)
        .IsInEnum();
    }
  }
}
