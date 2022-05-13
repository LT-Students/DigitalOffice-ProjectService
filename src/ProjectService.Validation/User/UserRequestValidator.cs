using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.User.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation.User
{
  public class UserRequestValidator : AbstractValidator<UserRequest>, IUserRequestValidator
  {
    public UserRequestValidator()
    {
      RuleFor(pu => pu.UserId)
        .NotEmpty()
        .WithMessage("Not specified user id.");

      RuleFor(pu => pu.Role)
        .IsInEnum();
    }
  }
}
