using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class ProjectUserValidator : AbstractValidator<ProjectUserRequest>, IProjectUserValidator
    {
        public ProjectUserValidator()
        {
            RuleFor(pu => pu)
                .Must(u => u != null)
                .WithMessage("The request must contain user data")
                .DependentRules(() =>
                {
                    RuleFor(pu => pu.Id)
                    .NotEmpty()
                    .WithMessage("Not specified user id.");

                    /*RuleFor(pu => pu.)
                       .NotEmpty()
                       .WithMessage("Not specified role id.");*/

                });
        }
    }
}
