using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class WorkersProjectValidator : AbstractValidator<ProjectUserRequest>
    {
        public WorkersProjectValidator()
        {
            RuleFor(pu => pu.User.Id)
                .NotEmpty()
                .WithMessage("Each user id");
        }
    }
}
