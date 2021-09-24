using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
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
      ITaskPropertyRepository taskPropertyRepository,
      IImageContentValidator imageContentValidator)
    {
      List<string> errors = new();

      RuleFor(task => task.Name)
        .NotEmpty()
        .MaximumLength(150)
        .WithMessage("Task name is too long.");

      When(task => !string.IsNullOrEmpty(task.Description?.Trim()), () =>
      {
        RuleFor(task => task.Description)
          .MaximumLength(300)
          .WithMessage("Task description is too long.");
      });

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

      When(project => project.TaskImages != null && project.TaskImages.Any(), () =>
      {
        RuleForEach(project => project.TaskImages)
          .Must(image => imageContentValidator.ValidateCustom(image, out errors))
          .WithMessage(errors[0]);
      });

      //When(task => task.AssignedTo.HasValue, () =>
      //{
      //    RuleFor(task => task)
      //        .Must(task => userRepository.AreUserProjectExist(task.AssignedTo.Value, task.ProjectId))
      //        .WithMessage("User does not exist.");
      //});

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
        .WithMessage("Status id does not exist.");

      RuleFor(task => task.TypeId)
        .NotEmpty()
        .Must(x => taskPropertyRepository.AreExist(x))
        .WithMessage("Type id does not exist.");
    }
  }
}
