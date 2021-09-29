using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.ProjectService.Validation
{
  public class EditTaskPropertyValidator : AbstractValidator<JsonPatchDocument<TaskProperty>>, IEditTaskPropertyValidator
  {
    private readonly ITaskPropertyRepository _taskPropertyRepository;
    private readonly IUserRepository _userRepository;

    public EditTaskPropertyValidator(
      ITaskPropertyRepository taskPropertyRepository,
      IUserRepository userRepository)
    {
      _taskPropertyRepository = taskPropertyRepository;
      _userRepository = userRepository;

      RuleForEach(x => x.Operations)
        .Custom(HandleInternalPropertyValidation);
    }

    private void HandleInternalPropertyValidation(Operation<TaskProperty> requestedOperation, CustomContext context)
    {
      #region local functions

      void AddСorrectPaths(List<string> paths)
      {
        if (paths.FirstOrDefault(p => p.EndsWith(requestedOperation.path[1..], StringComparison.OrdinalIgnoreCase)) == null)
        {
          context.AddFailure(requestedOperation.path, $"This path {requestedOperation.path} is not available");
        }
      }

      void AddСorrectOperations(
        string propertyName,
        List<OperationType> types)
      {
        if (requestedOperation.path.EndsWith(propertyName, StringComparison.OrdinalIgnoreCase)
          && !types.Contains(requestedOperation.OperationType))
        {
          context.AddFailure(propertyName, $"This operation {requestedOperation.OperationType} is prohibited for {propertyName}");
        }
      }

      void AddFailureForPropertyIf(
        string propertyName,
        Func<OperationType, bool> type,
        Dictionary<Func<Operation<TaskProperty>, bool>, string> predicates)
      {
        if (!requestedOperation.path.EndsWith(propertyName, StringComparison.OrdinalIgnoreCase)
          || !type(requestedOperation.OperationType))
        {
          return;
        }

        foreach (KeyValuePair<Func<Operation<TaskProperty>, bool>, string> validateDelegate in predicates)
        {
          if (!validateDelegate.Key(requestedOperation))
          {
            context.AddFailure(propertyName, validateDelegate.Value);
          }
        }
      }

      #endregion

      #region paths

      AddСorrectPaths(
        new List<string>
        {
          nameof(TaskProperty.Name),
          nameof(TaskProperty.PropertyType),
          nameof(TaskProperty.Description),
        });

      AddСorrectOperations(nameof(TaskProperty.Name), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(TaskProperty.PropertyType), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(TaskProperty.Description), new List<OperationType> { OperationType.Replace });

      #endregion

      #region firstname

      AddFailureForPropertyIf(
          nameof(TaskProperty.Name),
          x => x == OperationType.Replace || x == OperationType.Add,
          new()
          {
            { x => !string.IsNullOrEmpty(x.value.ToString()), "Name is empty." },
            { x => x.value.ToString().Length < 150, "Name is too long" }
          });

      #endregion

      #region description

      AddFailureForPropertyIf(
          nameof(TaskProperty.Description),
          x => x == OperationType.Replace || x == OperationType.Add,
          new()
          {
            {
              x =>
              {
                if (string.IsNullOrEmpty(x.ToString()))
                {
                  return true;
                }

                return x.value.ToString().Length < 150;
              },
              "Name is too long"
            }
          });

      #endregion

      #region PropertyType

      AddFailureForPropertyIf(
          nameof(TaskProperty.PropertyType),
          x => x == OperationType.Replace || x == OperationType.Add,
          new()
          {
            { x => Enum.IsDefined(typeof(TaskPropertyType), Convert.ToInt32(x.value)), "This PropertyType does not exist." }
          });

      #endregion
    }
  }
}
