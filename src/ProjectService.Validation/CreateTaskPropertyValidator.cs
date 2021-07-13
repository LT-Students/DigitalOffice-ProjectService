using FluentValidation;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class CreateTaskPropertyValidator : AbstractValidator<CreateTaskPropertyRequest>, ICreateTaskPropertyValidator
    {
        public CreateTaskPropertyValidator(ITaskPropertyRepository repository)
        {
            RuleFor(request => request.ProjectId)
                .NotEmpty()
                .DependentRules(() =>
                {
                    RuleForEach(request => request.TaskProperties).ChildRules(tp =>
                    {
                        tp.RuleFor(tp => tp.Name)
                            .NotEmpty()
                            .MaximumLength(32);

                        tp.RuleFor(tp => tp.PropertyType)
                            .IsInEnum();

                        tp.When(tp => !string.IsNullOrEmpty(tp.Description.Trim()), () =>
                        {
                            tp.RuleFor(tp => tp.Description)
                                .MaximumLength(300)
                                .WithMessage("Task property description is too long.");
                        });
                    })
                    .DependentRules(() =>
                    {
                        RuleFor(request => request)
                        .Must(request => !repository.AreExistForProject(
                            request.ProjectId,
                            request.TaskProperties.Select(x => x.Name).ToArray()))
                        .WithMessage("One of the task property name already exist.");
                    });
                });
        }
    }
}
