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
        public CreateTaskRequestValidator(
            ITaskRepository tasksRepository,
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            ITaskPropertyRepository taskPropertyRepository)

        {
            RuleFor(task => task.Name)
                .NotEmpty()
                .MaximumLength(150)
                .WithMessage("Task name is too long.");

            RuleFor(task => task.Description)
                .NotEmpty()
                .WithMessage("Task must have description");

            When(task => task.ParentId.HasValue, () =>
            {
                RuleFor(task => task.ParentId)
                    .Must(x => tasksRepository.IsExist(x.Value))
                    .WithMessage("Task does not exist");
            });

            When(task => task.AssignedTo.HasValue, () =>
            {
                RuleFor(task => task)
                    .Must(task => userRepository.AreUserProjectExist(task.AssignedTo.Value, task.ProjectId))
                    .WithMessage("User does not exist");
            });

            RuleFor(task => task.ProjectId)
                .NotEmpty()
                .Must(x => projectRepository.IsExist(x))
                .WithMessage("Project does not exist");

            RuleFor(task => task.PriorityId)
                .NotEmpty()
                .Must(x => taskPropertyRepository.AreExist(x));

            RuleFor(task => task.StatusId)
                .NotEmpty()
                .Must(x => taskPropertyRepository.AreExist(x));

            RuleFor(task => task.TypeId)
                .NotEmpty()
                .Must(x => taskPropertyRepository.AreExist(x));
        }
    }
}
