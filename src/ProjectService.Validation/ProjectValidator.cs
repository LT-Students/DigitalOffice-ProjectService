using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

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

            When(project => project.ShortName != null, () =>
            {
                RuleFor(project => project.ShortName)
                    .MaximumLength(32)
                    .WithMessage("Project short name is too long.");
            });

            When(project => project.Description != null, () =>
            {
                RuleFor(project => project.Description)
                .MaximumLength(500)
                .WithMessage("Project description is too long.");
            });
        }
    }
}