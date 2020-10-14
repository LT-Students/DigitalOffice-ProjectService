using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class ProjectValidator : AbstractValidator<NewProjectRequest>
    {
        public ProjectValidator()
        {
            RuleFor(project => project.Name)
                    .NotEmpty()
                    .WithMessage("Project must have a name.")
                    .MaximumLength(80)
                    .WithMessage("Project name is too long.");

            RuleFor(project => project.ShortName)
                    .NotEmpty()
                    .WithMessage("Project must have a short name.")
                    .MaximumLength(32)
                    .WithMessage("Project short name is too long.");

            RuleFor(project => project.Description)
                .MaximumLength(500)
                .WithMessage("Project description is too long.");
        }
    }
}