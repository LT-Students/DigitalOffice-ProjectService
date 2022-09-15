using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
using LT.DigitalOffice.ProjectService.Validation.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Validation.Project.Resources;
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

      Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

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
          { x => Enum.TryParse(typeof(ProjectStatusType), x.value?.ToString(), true, out _), ProjectRequestValidationResource.IncorrectStatus }
        });

      #endregion

      #region Name

      await AddFailureForPropertyIfAsync(
        nameof(EditProjectRequest.Name),
        x => x == OperationType.Replace,
        new()
        {
          { x => Task.FromResult(!string.IsNullOrEmpty(x.value?.ToString().Trim())), string.Join(' ', nameof(EditProjectRequest.Name), ProjectRequestValidationResource.NameNotNullOrEmpty) },
          { x => Task.FromResult(x.value.ToString().Trim().Length < 151), ProjectRequestValidationResource.NameLong },
          { async x => !await _projectRepository.DoesNameExistAsync(x.value?.ToString()?.Trim()), ProjectRequestValidationResource.NameExists }
        }, CascadeMode.Stop);

      #endregion

      #region ShortName

      await AddFailureForPropertyIfAsync(
        nameof(EditProjectRequest.ShortName),
        x => x == OperationType.Replace,
        new ()
        {
          { x => Task.FromResult(x.value.ToString().Trim().Length < 41), ProjectRequestValidationResource.ShortNameLong },
          { async x => !await _projectRepository.DoesShortNameExistAsync(x.value?.ToString()?.Trim()), ProjectRequestValidationResource.ShortNameExists }
        });

      #endregion

      #region ShortDescription

      AddFailureForPropertyIf(
        nameof(EditProjectRequest.ShortDescription),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditProjectRequest>, bool>, string>
        {
          { x => x.value == null || x.value.ToString().Trim().Length < 301, ProjectRequestValidationResource.ShortDescriptionLong },
        });

      #endregion

      #region Customer

      AddFailureForPropertyIf(
        nameof(EditProjectRequest.Customer),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditProjectRequest>, bool>, string>
        {
          { x => x.value == null || x.value.ToString().Trim().Length < 151, ProjectRequestValidationResource.CustomerLong },
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
