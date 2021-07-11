using FluentValidation;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;

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
                .WithMessage("Task must have description.");

            When(task => task.ParentId.HasValue, () =>
            {
                DbTask parentTask = null;

                RuleFor(task => task.ParentId)
                    .Must(x =>
                        {
                            parentTask = tasksRepository.Get(x.Value, false);
                            return parentTask != null;
                        })
                    .WithMessage("Task does not exist.")
                    .Must(_ =>
                    {
                        return parentTask?.ParentId == null;
                    })
                    .WithMessage("Parent task must have not to have a parent.");
            });

            When(task => task.AssignedTo.HasValue, () =>
            {
                RuleFor(task => task)
                    .Must(task => userRepository.AreUserProjectExist(task.AssignedTo.Value, task.ProjectId))
                    .WithMessage("User does not exist.");
            });

            RuleFor(task => task.ProjectId)
                .NotEmpty()
                .Must(x => projectRepository.IsExist(x))
                .WithMessage("Project does not exist.");

            RuleFor(task => task.PriorityId)
                .NotEmpty()
                .Must(x => taskPropertyRepository.AreExist(x))
                .WithMessage("Priority id does not exist.");

            RuleFor(task => task.StatusId)
                .NotEmpty()
                .Must(x => taskPropertyRepository.AreExist(x))
                .WithMessage("Status id not exist.");

            RuleFor(task => task.TypeId)
                .NotEmpty()
                .Must(x => taskPropertyRepository.AreExist(x))
                .WithMessage("Type id not exist.");
        }
    }
}
