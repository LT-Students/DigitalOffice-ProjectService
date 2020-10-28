using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class EditProjectValidator : AbstractValidator<EditProjectRequest>
    {
        public EditProjectValidator()
        {
            RuleFor(project => project.Id)
                .NotEmpty()
                .WithMessage("Request must have a project Id");

            RuleFor(project => project.Name)
                    .NotEmpty()
                    .WithMessage("Project must have a name.")
                    .MaximumLength(80)
                    .WithMessage("Project name is too long.");

            RuleFor(project => project.Description)
                .MaximumLength(500)
                .WithMessage("Project description is too long.");
        }
    }
}