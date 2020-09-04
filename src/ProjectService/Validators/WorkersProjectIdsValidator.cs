using FluentValidation;
using LT.DigitalOffice.ProjectService.Models;

namespace LT.DigitalOffice.ProjectService.Validators
{
    public class WorkersProjectIdsValidator : AbstractValidator<WorkersIdsInProjectRequest>
    {
        public WorkersProjectIdsValidator()
        {
            RuleFor(workers => workers.ProjectId)
                .NotEmpty()
                .WithMessage("Project id");

            RuleForEach(workers => workers.WorkersIds)
                .NotEmpty()
                .WithMessage("Each worker id");
        }
    }
}
