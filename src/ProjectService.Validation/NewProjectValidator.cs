using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class NewProjectValidator : AbstractValidator<NewProjectRequest>
    {
        public NewProjectValidator()
        {
            RuleFor(project => project.Name)
                    .NotEmpty()
                    .WithMessage("Project must have a name.")
                    .MaximumLength(80)
                    .WithMessage("Project name is too long.");
        }
    }
}