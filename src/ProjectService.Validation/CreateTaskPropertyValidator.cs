using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    .NotEmpty()
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
