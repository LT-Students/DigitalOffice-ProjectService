using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation
{
  public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>, ICreateTaskValidator
  {
    private readonly List<string> imageFormats = new()
    {
      ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tga"
    };

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
          .Must(x => !string.IsNullOrEmpty(x.Content))
          .WithMessage("Content can't be empty.")
          .Must(x => imageFormats.Contains(x.Extension))
          .WithMessage("Wrong extension.")
          .Must(images => images.Name.Length < 150)
          .WithMessage("Name's length must be less than 150 letters.")
          .Must(images =>
          {
            try
            {
              var byteString = new Span<byte>(new byte[images.Content.Length]);
              return Convert.TryFromBase64String(images.Content, byteString, out _);
            }
            catch
            {
              return false;
            }
          }).WithMessage("Wrong image content.");
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
