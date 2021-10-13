using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
  public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>, ICreateProjectRequestValidator
  {
    private readonly ILogger<CreateProjectRequestValidator> _logger;
    private readonly IRequestClient<ICheckDepartmentsExistence> _rcCheckDepartmentsExistence;
    private readonly IRequestClient<ICheckUsersExistence> _rcCheckUsersExistence;

    public CreateProjectRequestValidator(
      IProjectRepository projectRepository,
      ILogger<CreateProjectRequestValidator> logger,
      IRequestClient<ICheckDepartmentsExistence> rcCheckDepartmentsExistence,
      IRequestClient<ICheckUsersExistence> rcCheckUsersExistence,
      IImageValidator imageValidator)
    {
      _logger = logger;
      _rcCheckDepartmentsExistence = rcCheckDepartmentsExistence;
      _rcCheckUsersExistence = rcCheckUsersExistence;

      RuleFor(project => project.Name.Trim())
        .Cascade(CascadeMode.Stop)
        .NotEmpty().WithMessage("Project name must not be empty.")
        .MaximumLength(150).WithMessage("Project name is too long.")
        .Must(name => !projectRepository.DoesProjectNameExist(name))
        .WithMessage(project => $"Project with name '{project.Name}' already exists.");

      RuleFor(project => project.Status)
        .IsInEnum();

      When(project => !string.IsNullOrEmpty(project.ShortName?.Trim()), () =>
      {
        RuleFor(project => project.ShortName)
          .MaximumLength(30)
          .WithMessage("Project short name is too long.");
      });

      When(project => !string.IsNullOrEmpty(project.ShortDescription?.Trim()), () =>
      {
        RuleFor(project => project.ShortDescription)
          .MaximumLength(300)
          .WithMessage("Project short description is too long.");
      });

      When(project => project.DepartmentId.HasValue, () =>
      {
        RuleFor(project => project.DepartmentId)
          .Cascade(CascadeMode.Stop)
          .Must(departmentId => departmentId != Guid.Empty)
          .WithMessage("Wrong type of department Id.")
          .MustAsync(async (id, cancellation) => await CheckValidityDepartmentId(id))
          .WithMessage("Some departments does not exist.");
      });

      When(project => project.Users != null && project.Users.Any(), () =>
      {
        RuleForEach(project => project.Users)
          .ChildRules(user =>
          {
            user.RuleFor(user => user.UserId)
              .NotEmpty().WithMessage("Wrong type of user Id.");

            user.RuleFor(user => user.Role)
              .IsInEnum();
          });

        RuleFor(project => project.Users)
          .Cascade(CascadeMode.Stop)
          .Must(p => p.Select(pu => pu.UserId).Distinct().Count() == p.Count())
          .WithMessage("User cannot be added to the project twice.")
          .MustAsync(async (pu, cancellation) => await CheckValidityUsersIds(pu.Select(u => u.UserId).ToList()))
          .WithMessage("Some users does not exist.");
      });

      When(project => project.ProjectImages != null && project.ProjectImages.Any(), () =>
      {
        RuleForEach(project => project.ProjectImages)
          .SetValidator(imageValidator);
      });
    }

    private async Task<bool> CheckValidityDepartmentId(Guid? departmentId)
    {
      if (!departmentId.HasValue)
      {
        return true;
      }

      string logMessage = "Department with id: {id} not found.";

      try
      {
        var response = await _rcCheckDepartmentsExistence.GetResponse<IOperationResult<ICheckDepartmentsExistence>>(
          ICheckDepartmentsExistence.CreateObj(new List<Guid> { departmentId.Value }));
        if (response.Message.IsSuccess)
        {
          return response.Message.Body.DepartmentIds.Any();
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

    private async Task<bool> CheckValidityUsersIds(List<Guid> usersIds)
    {
      if (!usersIds.Any())
      {
        return true;
      }

      string logMessage = "Cannot check existing users withs this ids {userIds}";

      try
      {
        var response = await _rcCheckUsersExistence.GetResponse<IOperationResult<ICheckUsersExistence>>(
          ICheckUsersExistence.CreateObj(usersIds));
        if (response.Message.IsSuccess)
        {
          return response.Message.Body.UserIds.Count() == usersIds.Count();
        }

        _logger.LogWarning($"Can not find with this Ids: {usersIds}: {Environment.NewLine}{string.Join('\n', response.Message.Errors)}");
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage);
      }

      return false;
    }
  }
}
