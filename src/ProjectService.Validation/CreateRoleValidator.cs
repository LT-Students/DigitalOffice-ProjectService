using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleRequest>
    {
        public CreateRoleValidator()
        {
            RuleFor(rule => rule.Name)
                    .NotEmpty()
                    .WithMessage("Role must have a name.")
                    .MaximumLength(32)
                    .WithMessage("Role name is too long.");

            RuleFor(rule => rule.Description)
                    .MaximumLength(512)
                    .WithMessage("Role description is too long.");
        }
    }
}