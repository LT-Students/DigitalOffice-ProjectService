using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Validation
{
  public class EditProjectRequestValidator : BaseEditRequestValidator<EditProjectRequest>, IEditProjectRequestValidator
  {
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<EditProjectRequestValidator> _logger;
    private readonly IRequestClient<ICheckDepartmentsExistence> _rcDepartmentsExistence;

    private void HandleInternalPropertyValidation(Operation<EditProjectRequest> requestedOperation, CustomContext context)
    {
      Context = context;
      RequestedOperation = requestedOperation;

      #region paths

      AddСorrectPaths(
        new List<string>
        {
          nameof(EditProjectRequest.Status),
          nameof(EditProjectRequest.DepartmentId),
          nameof(EditProjectRequest.Name),
          nameof(EditProjectRequest.ShortName),
          nameof(EditProjectRequest.Description),
          nameof(EditProjectRequest.ShortDescription)
        });

      AddСorrectOperations(nameof(EditProjectRequest.Status), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditProjectRequest.DepartmentId), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditProjectRequest.Name), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditProjectRequest.ShortName), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditProjectRequest.Description), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditProjectRequest.ShortDescription), new List<OperationType> { OperationType.Replace });
      #endregion

      #region status

      AddFailureForPropertyIf(
        nameof(EditProjectRequest.Status),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditProjectRequest>, bool>, string>
        {
          { x => !Enum.TryParse(typeof(ProjectStatusType), x.value?.ToString(), true, out _), "Incorrect project status." }
        });

      #endregion

      #region DepartmentId

      AddFailureForPropertyIf(
        nameof(EditProjectRequest.DepartmentId),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditProjectRequest>, bool>, string>
        {
          { x => x.value == null ||
            (Guid.TryParse(x.value.ToString(), out Guid departmentId) && CheckValidityDepartmentId(departmentId)),
            "Incorrect department id value." },
        });

      #endregion

      #region Name

      AddFailureForPropertyIf(
        nameof(EditProjectRequest.Name),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditProjectRequest>, bool>, string>
        {
          { x => !string.IsNullOrEmpty(x.value?.ToString().Trim()), "Name must not be empty." },
          { x => x.value.ToString().Trim().Length < 150, "Name is too long." },
          { x => !_projectRepository.DoesProjectNameExist(x.value.ToString().Trim()), "The project name already exist." }
        }, CascadeMode.Stop);

      #endregion

      #region ShortName

      AddFailureForPropertyIf(
        nameof(EditProjectRequest.ShortName),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditProjectRequest>, bool>, string>
        {
          { x => x.value == null || x.value.ToString().Trim().Length < 30, "Short name is too long." },
        });

      #endregion

      #region ShortDescription

      AddFailureForPropertyIf(
        nameof(EditProjectRequest.ShortName),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditProjectRequest>, bool>, string>
        {
          { x => x.value == null || x.value.ToString().Trim().Length < 300, "Short description is too long." },
        });

      #endregion
    }

    public EditProjectRequestValidator(
      IProjectRepository projectRepository,
      ILogger<EditProjectRequestValidator> logger,
      IRequestClient<ICheckDepartmentsExistence> rcDepartmentsExistence)
    {
      _projectRepository = projectRepository;
      _logger = logger;
      _rcDepartmentsExistence = rcDepartmentsExistence;

      RuleForEach(x => x.Operations)
        .Custom(HandleInternalPropertyValidation);
    }

    private bool CheckValidityDepartmentId(Guid departmentId)
    {
      if (departmentId == Guid.Empty)
      {
        return false;
      }

      try
      {
        Response<IOperationResult<ICheckDepartmentsExistence>> response =
          _rcDepartmentsExistence.GetResponse<IOperationResult<ICheckDepartmentsExistence>>(
            ICheckDepartmentsExistence.CreateObj(new() { departmentId })).Result;

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.DepartmentIds.Any();
        }

        _logger.LogWarning(
          "Error while checking department existence with id {departmentId}.\n Errors: {Errors}",
          departmentId,
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(
          exc,
          "Cannot check department existence with id {departmentId}.",
          departmentId);
      }

      return false;
    }
  }
}
