using FluentValidation;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>, ICreateTaskValidator
    {
        public CreateTaskRequestValidator()
        {
            RuleFor(task => task.Name)
                .NotEmpty()
                .MaximumLength(150).WithMessage("Task name is too long.");

            RuleFor(task => task.Description)
                .NotEmpty()
                .WithMessage("Task must have description");

            When(task => task.Deadline != null, () =>
            {
                RuleFor(task => task.Deadline)
                    .GreaterThan(task => task.CreatedAt)
                .WithMessage("Dedline should be late then created time");
            });
        }
    }
}
