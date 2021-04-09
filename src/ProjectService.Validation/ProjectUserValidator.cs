using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class ProjectUserValidator : AbstractValidator<ProjectUserRequest>, IProjectUserValidator
    {
        public ProjectUserValidator()
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
