using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Project.Interfaces;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.ProjectService.Validation.Project
{
  public class EditProjectRequestValidator : BaseEditRequestValidator<EditProjectRequest>, IEditProjectRequestValidator
  {
    private readonly IProjectRepository _projectRepository;

    private async Task HandleInternalPropertyValidationAsync(Operation<EditProjectRequest> requestedOperation, CustomContext context)
    {
      Context = context;
      RequestedOperation = requestedOperation;

      #region paths

      AddСorrectPaths(
        new List<string>
        {
          nameof(EditProjectRequest.Status),
          nameof(EditProjectRequest.Name),
          nameof(EditProjectRequest.ShortName),
          nameof(EditProjectRequest.Description),
          nameof(EditProjectRequest.ShortDescription),
          nameof(EditProjectRequest.Customer),
          nameof(EditProjectRequest.StartDateUtc),
          nameof(EditProjectRequest.EndDateUtc)
        });

      AddСorrectOperations(nameof(EditProjectRequest.Status), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditProjectRequest.Name), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditProjectRequest.ShortName), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditProjectRequest.Description), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditProjectRequest.ShortDescription), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditProjectRequest.Customer), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditProjectRequest.StartDateUtc), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditProjectRequest.EndDateUtc), new List<OperationType> { OperationType.Replace });

      #endregion

      #region status

      AddFailureForPropertyIf(
        nameof(EditProjectRequest.Status),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditProjectRequest>, bool>, string>
        {
          { x => Enum.TryParse(typeof(ProjectStatusType), x.value?.ToString(), true, out _), "Incorrect project status." }
        });

      #endregion

      #region Name

      await AddFailureForPropertyIfAsync(
        nameof(EditProjectRequest.Name),
        x => x == OperationType.Replace,
        new()
        {
          { x => Task.FromResult(!string.IsNullOrEmpty(x.value?.ToString().Trim())), "Name must not be empty." },
          { x => Task.FromResult(x.value.ToString().Trim().Length < 151), "Name is too long." },
          { async x => !await _projectRepository.DoesNameExistAsync(x.value?.ToString()?.Trim()), "The project name already exist." }
        }, CascadeMode.Stop);

      #endregion

      #region ShortName

      await AddFailureForPropertyIfAsync(
        nameof(EditProjectRequest.ShortName),
        x => x == OperationType.Replace,
        new ()
        {
          { x => Task.FromResult(x.value.ToString().Trim().Length < 41), "Short name is too long." },
          { async x => !await _projectRepository.DoesShortNameExistAsync(x.value?.ToString()?.Trim()), "The project short name already exist." }
        });

      #endregion

      #region ShortDescription

      AddFailureForPropertyIf(
        nameof(EditProjectRequest.ShortName),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditProjectRequest>, bool>, string>
        {
          { x => x.value == null || x.value.ToString().Trim().Length < 301, "Short description is too long." },
        });

      #endregion

      #region Customer

      AddFailureForPropertyIf(
        nameof(EditProjectRequest.Customer),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditProjectRequest>, bool>, string>
        {
          { x => x.value == null || x.value.ToString().Trim().Length < 151, "Customer is too long." },
        });

      #endregion
    }

    public EditProjectRequestValidator(
      IProjectRepository projectRepository)
    {
      _projectRepository = projectRepository;

      RuleForEach(x => x.Operations)
        .CustomAsync(async (x, context, _) => await HandleInternalPropertyValidationAsync(x, context));
    }
  }
}
