using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class AddUsersToProjectValidator : AbstractValidator<AddUsersToProjectRequest>
    {
        public AddUsersToProjectValidator()
        {
            RuleFor(project => project.ProjectId)
               .NotEmpty()
               .WithMessage("Request must have a project Id");

            When(project => project.Users != null && project.Users.Count() != 0, () =>
            {
                RuleForEach(project => project.Users).SetValidator(new ProjectUserRequestValidator());
            });
        }
    }
}
