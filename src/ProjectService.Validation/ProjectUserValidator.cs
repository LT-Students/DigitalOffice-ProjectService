using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class ProjectUserValidator : AbstractValidator<ProjectUserRequest>, IProjectUserValidator
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
