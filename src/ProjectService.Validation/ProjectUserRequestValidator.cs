using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class ProjectUserRequestValidator : AbstractValidator<ProjectUserRequest>
    {
        public ProjectUserRequestValidator()
        {
            RuleFor(pu => pu.User)
                .Must(u => u != null)
                .WithMessage("The request must contain user data")
                .DependentRules(() =>
                {
                    RuleFor(pu => pu.User.Id)
                    .NotEmpty()
                    .WithMessage("Not specified user id.");

                    RuleFor(pu => pu.RoleId)
                       .NotEmpty()
                       .WithMessage("Not specified role id.");

                });
        }
    }
}
