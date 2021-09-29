using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.ProjectService.Validation
{
  public class EditTaskPropertyValidator : BaseEditRequestValidator<TaskProperty>, IEditTaskPropertyValidator
  {
    public EditTaskPropertyValidator()
    {
      RuleForEach(x => x.Operations)
        .Custom(HandleInternalPropertyValidation);
    }

    private void HandleInternalPropertyValidation(Operation<TaskProperty> requestedOperation, CustomContext context)
    {
      Context = context;
      RequestedOperation = requestedOperation;

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
          x => x == OperationType.Replace,
          new()
          {
            { x => !string.IsNullOrEmpty(x.value.ToString()), "Name must not be empty." },
            { x => x.value.ToString().Trim().Length < 150, "Name is too long" }
          });

      #endregion

      #region description

      AddFailureForPropertyIf(
          nameof(TaskProperty.Description),
          x => x == OperationType.Replace,
          new()
          {
            {
              x =>
              {
                if (string.IsNullOrEmpty(x.ToString()))
                {
                  return true;
                }

                return x.value.ToString().Trim().Length < 150;
              },
              "Description is too long"
            }
          });

      #endregion

      #region PropertyType

      AddFailureForPropertyIf(
          nameof(TaskProperty.PropertyType),
          x => x == OperationType.Replace,
          new()
          {
            { x => Enum.IsDefined(typeof(TaskPropertyType), Convert.ToInt32(x.value)), "This PropertyType does not exist." }
          });

      #endregion
    }
  }
}
