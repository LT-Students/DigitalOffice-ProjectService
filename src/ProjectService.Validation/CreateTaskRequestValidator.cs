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
        public CreateTaskRequestValidator(ITaskRepository tasksRepository, IUserRepository userRepository, IProjectRepository projectRepository)
        {
            RuleFor(task => task.Name)
                .NotEmpty()
                .MaximumLength(150).WithMessage("Task name is too long.");

            RuleFor(task => task.Description)
                .NotEmpty()
                .WithMessage("Task must have description");

            When(task => task.ParentTaskId.HasValue, () =>
            {
                RuleFor(task => task.ParentTaskId)
                    .Must(x => tasksRepository.IsExist(x.Value));
            });

            When(task => task.AssignedTo.HasValue, () =>
            {
                RuleFor(task => task)
                    .Must(task => userRepository.AreUserAndProjectExist(task.AssignedTo.Value, task.ProjectId))
                    .WithMessage("User does not exist");
            });

            RuleFor(task => task.ProjectId)
                .NotEmpty()
                .Must(x => projectRepository.IsExist(x));

            When(task => task.Deadline != null, () =>
            {
                RuleFor(task => task.Deadline)
                    .GreaterThan(task => task.CreatedAt)
                .WithMessage("Dedline should be late then created time");
            });
        }
    }
}
