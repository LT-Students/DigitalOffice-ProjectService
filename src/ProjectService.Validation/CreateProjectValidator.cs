using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class CreateProjectValidator : AbstractValidator<ProjectRequest>, ICreateProjectValidator
    {
        public CreateProjectValidator()
        {
            RuleFor(project => project.DepartmentId)
                .NotEmpty();

            RuleFor(project => project.Name)
                .NotEmpty()
                .MaximumLength(150)
                .WithMessage("Project name is too long.");

            RuleFor(project => project.ShortName)
                .NotEmpty()
                .MaximumLength(30)
                .WithMessage("Project short name is too long.");

            RuleFor(project => project.Status)
                .IsInEnum();

            When(project => project.Users != null && project.Users.Any(), () =>
            {
                RuleForEach(project => project.Users).ChildRules(user =>
                {
                    user.RuleFor(user => user.UserId)
                        .NotEmpty();

                    user.RuleFor(user => user.Role)
                        .IsInEnum();
                });
            });

            When(project => project.ShortDescription != null, () =>
            {
                RuleFor(project => project.Description)
                    .MaximumLength(300)
                    .WithMessage("Project short description is too long.");
            });
        }
    }
}