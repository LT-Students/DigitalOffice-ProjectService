using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.ProjectService.Validation
{
  public class EditTaskPropertyValidator : BaseEditRequestValidator<EditTaskPropertyRequest>, IEditTaskPropertyValidator
  {
    private readonly IProjectRepository _projectRepository;
    public EditTaskPropertyValidator(
      IProjectRepository projectRepository)
    {
      _projectRepository = projectRepository;

      RuleForEach(x => x.Operations)
        .Custom(HandleInternalPropertyValidation);
    }

    private void HandleInternalPropertyValidation(Operation<EditTaskPropertyRequest> requestedOperation, CustomContext context)
    {
      Context = context;
      RequestedOperation = requestedOperation;

      #region paths

      AddСorrectPaths(
        new List<string>
        {
          nameof(EditTaskPropertyRequest.Name),
          nameof(EditTaskPropertyRequest.PropertyType),
          nameof(EditTaskPropertyRequest.Description),
          nameof(EditTaskPropertyRequest.ProjectId),
          nameof(EditTaskPropertyRequest.IsActive)
        });

      AddСorrectOperations(nameof(EditTaskPropertyRequest.Name), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditTaskPropertyRequest.PropertyType), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditTaskPropertyRequest.Description), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditTaskPropertyRequest.ProjectId), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditTaskPropertyRequest.IsActive), new List<OperationType> { OperationType.Replace });

      #endregion

      #region name

      AddFailureForPropertyIf(
          nameof(EditTaskPropertyRequest.Name),
          x => x == OperationType.Replace,
          new()
          {
            { x => !string.IsNullOrEmpty(x.value?.ToString()), "Name must not be empty." },
            { x => x.value?.ToString().Trim().Length < 150, "Name is too long." }
          });

      #endregion

      #region description

      AddFailureForPropertyIf(
          nameof(EditTaskPropertyRequest.Description),
          x => x == OperationType.Replace,
          new()
          {
            {
              x =>
              {
                if (string.IsNullOrEmpty(x.value?.ToString()))
                {
                  return true;
                }

                return x.value.ToString().Trim().Length < 150;
              },
              "Description is too long."
            }
          });

      #endregion

      #region PropertyType

      AddFailureForPropertyIf(
          nameof(EditTaskPropertyRequest.PropertyType),
          x => x == OperationType.Replace,
          new()
          {
            { x => Enum.IsDefined(typeof(TaskPropertyType), x.value?.ToString()), "This PropertyType does not exist." }
          });

      #endregion

      #region ProjectId

      AddFailureForPropertyIf(
          nameof(EditTaskPropertyRequest.ProjectId),
          x => x == OperationType.Replace,
          new()
          {
            { x => Guid.TryParse(x.value.ToString(), out Guid result), "Project id has incorrect format." },
            { x => _projectRepository.IsExist(Guid.Parse(x.value?.ToString())), "Project id does not exist." }
          });

      #endregion

      #region IsActive

      AddFailureForPropertyIf(
          nameof(EditTaskPropertyRequest.IsActive),
          x => x == OperationType.Replace,
          new()
          {
            { x => bool.TryParse(x.value?.ToString(), out bool result), "Incorrect taskProperty is active format." }
          });

      #endregion
    }
  }
}
