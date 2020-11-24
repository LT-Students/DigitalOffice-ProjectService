using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class WorkersProjectValidator : AbstractValidator<ProjectUser>
    {
        public WorkersProjectValidator()
        {
            RuleFor(pu => pu.User.Id)
                .NotEmpty()
                .WithMessage("Each user id");
        }
    }
}
