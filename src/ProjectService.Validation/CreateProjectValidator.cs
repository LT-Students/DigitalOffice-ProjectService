using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Validation
{
  public class CreateProjectValidator : AbstractValidator<CreateProjectRequest>, ICreateProjectValidator
  {
    private readonly ILogger<CreateProjectValidator> _logger;
    private readonly IRequestClient<ICheckDepartmentsExistence> _rcCheckDepartmentsExistence;
    private readonly IRequestClient<ICheckUsersExistence> _rcCheckUsersExistence;

    private readonly List<string> imageFormats = new()
    {
      ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tga"
    };

    public CreateProjectValidator(
      IProjectRepository projectRepository,
      ILogger<CreateProjectValidator> logger,
      IRequestClient<ICheckDepartmentsExistence> rcCheckDepartmentsExistence,
      IRequestClient<ICheckUsersExistence> rcCheckUsersExistence)
    {
      _logger = logger;
      _rcCheckDepartmentsExistence = rcCheckDepartmentsExistence;
      _rcCheckUsersExistence = rcCheckUsersExistence;

      RuleFor(project => project)
        .NotEmpty()
        .Must(project => !projectRepository.DoesProjectNameExist(project.Name))
        .WithMessage(project => $"Project with name '{project.Name}' already exists.")
        .Must(project => CheckValidityDepartmentId(project.DepartmentId))
        .WithMessage("Some departments does not exist.")
        .Must(project => CheckValidityUsersIds(project.Users.Select(u => u.UserId).ToList()))
        .WithMessage("Some users does not exist.");

      RuleFor(project => project.Name)
        .NotEmpty()
        .MaximumLength(150)
        .WithMessage("Project name is too long.");

      RuleFor(project => project.Status)
        .IsInEnum();

      When(project => !string.IsNullOrEmpty(project.ShortName?.Trim()), () =>
      {
        RuleFor(project => project.ShortName)
          .MaximumLength(30)
          .WithMessage("Project short name is too long.");
      });

      When(project => project.Users != null && project.Users.Any(), () =>
      {
        RuleForEach(project => project.Users).ChildRules(user =>
          {
            user.RuleFor(user => user.UserId)
              .NotEmpty();

            user.RuleFor(user => user.Role)
              .IsInEnum();
          });
      });

      When(project => project.ProjectImages != null && project.ProjectImages.Any(), () =>
      {
        RuleForEach(project => project.ProjectImages)
          .Must(x => !string.IsNullOrEmpty(x.Content))
          .WithMessage("Content can't be empty")
          .Must(x => imageFormats.Contains(x.Extension))
          .WithMessage("Wrong extension")
          .Must(images => images.Name.Length < 150)
          .WithMessage("Name's length must be less than 150 letters")
          .Must(images =>
            {
              try
              {
                return Convert.TryFromBase64String(images.Content, new Span<byte>(new byte[images.Content.Length]), out _);
              }
              catch
              {
                return false;
              }
            }).WithMessage("Wrong image content.");
      });

      When(project => !string.IsNullOrEmpty(project.ShortDescription?.Trim()), () =>
      {
        RuleFor(project => project.ShortDescription)
          .MaximumLength(300)
          .WithMessage("Project short description is too long.");
      });

      When(project => !string.IsNullOrEmpty(project.Description?.Trim()), () =>
      {
        RuleFor(project => project.Description)
          .MaximumLength(300)
          .WithMessage("Project description is too long.");
      });

      When(
        news => news.DepartmentId.HasValue,
        () =>
          RuleFor(news => news.DepartmentId)
            .Must(DepartmentId => DepartmentId != Guid.Empty)
            .WithMessage("Wrong type of department Id."));
    }

    private bool CheckValidityDepartmentId(Guid? departmentId)
    {
      if (!departmentId.HasValue)
      {
        return true;
      }

      string logMessage = "Department with id: {id} not found.";

      try
      {
        Response<IOperationResult<ICheckDepartmentsExistence>> response = _rcCheckDepartmentsExistence.GetResponse<IOperationResult<ICheckDepartmentsExistence>>(
          ICheckDepartmentsExistence.CreateObj(new List<Guid> { departmentId.Value })).Result;
        if (response.Message.IsSuccess && !response.Message.Body.DepartmentIds.Any())
        {
          return true;
        }

        _logger.LogWarning("Can not find department. Reason: '{Errors}'",
          string.Join(',', response.Message.Errors));

        return false;
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage);
      }

      return false;
    }

    private bool CheckValidityUsersIds(List<Guid> userIds)
    {
      if (!userIds.Any())
      {
        return true;
      }

      string logMessage = "Cannot check existing users withs this ids {userIds}";

      try
      {
        Response<IOperationResult<ICheckUsersExistence>> response = _rcCheckUsersExistence.GetResponse<IOperationResult<ICheckUsersExistence>>(
          ICheckUsersExistence.CreateObj(userIds)).Result;
        if (response.Message.IsSuccess)
        {
          return true;
        }

        _logger.LogWarning($"Can not find with this Ids: {userIds}: {Environment.NewLine}{string.Join('\n', response.Message.Errors)}");
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage);
      }

      return false;
    }
  }
}
