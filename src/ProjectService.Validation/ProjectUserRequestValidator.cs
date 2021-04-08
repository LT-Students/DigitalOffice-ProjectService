using FluentValidation;
using FluentValidation.Results;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class ProjectUserRequestValidator : AbstractValidator<ProjectUserRequest>, IProjectUserRequestValidator
    {
        public ProjectUserRequestValidator()
        {
            RuleFor(pu => pu.User)
                .Must(u => u != null)
                .WithMessage("The request must contain user data")
                .DependentRules(() =>
                {
                    RuleFor(pu => pu.User.Id)
                    .NotEmpty()
                    .WithMessage("Not specified user id.");

                    RuleFor(pu => pu.RoleId)
                       .NotEmpty()
                       .WithMessage("Not specified role id.");

                });
        }
    }
}
