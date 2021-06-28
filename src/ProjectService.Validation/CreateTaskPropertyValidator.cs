using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class CreateTaskPropertyValidator : AbstractValidator<CreateTaskPropertyRequest>, ICreateTaskPropertyValidator
    {
        public CreateTaskPropertyValidator()
        {
            RuleFor(request => request.ProjectId)
                .NotEmpty();

            RuleForEach(request => request.TaskProperties).ChildRules(tp =>
            {
                tp.RuleFor(tp => tp.Name)
                    .NotEmpty()
                    .MaximumLength(32);

                tp.RuleFor(tp => tp.Name)
                    .NotEmpty()
                    .MaximumLength(32);

                tp.RuleFor(tp => tp.PropertyType)
                    .IsInEnum();

                tp.When(tp => tp.Description != null, () =>
                {
                    tp.RuleFor(tp => tp.Description)
                        .NotEmpty();
                });
            });
        }
    }
}
