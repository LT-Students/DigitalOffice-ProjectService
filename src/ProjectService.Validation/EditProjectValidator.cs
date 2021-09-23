﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Validation
{
  public class EditProjectValidator : AbstractValidator<JsonPatchDocument<EditProjectRequest>>, IEditProjectValidator
  {
    private readonly ILogger<CreateProjectValidator> _logger;
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartments;

    public static List<string> Paths =>
      new List<string>
      {
        Name,
        ShortName,
        Description,
        ShortDescription,
        Status,
        DepartmentId
      };

    public static string Name => $"/{nameof(EditProjectRequest.Name)}";
    public static string ShortName => $"/{nameof(EditProjectRequest.ShortName)}";
    public static string Description => $"/{nameof(EditProjectRequest.Description)}";
    public static string ShortDescription => $"/{nameof(EditProjectRequest.ShortDescription)}";
    public static string Status => $"/{nameof(EditProjectRequest.Status)}";
    public static string DepartmentId => $"/{nameof(EditProjectRequest.DepartmentId)}";
    Func<JsonPatchDocument<EditProjectRequest>, string, Operation> GetOperationByPath =>
      (x, path) =>
        x.Operations.FirstOrDefault(x =>
          string.Equals(x.path, path, StringComparison.OrdinalIgnoreCase));

    public EditProjectValidator(
      IProjectRepository projectRepository,
      ILogger<CreateProjectValidator> logger,
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments)
    {
      _logger = logger;
      _rcGetDepartments = rcGetDepartments;

      RuleFor(x => x.Operations)
        .Must(x => x.Select(x => x.path).Distinct().Count() == x.Count())
        .WithMessage("You don't have to change the same field of Project multiple times.")
        .Must(x => x.Any())
        .WithMessage("You don't have changes.")
        .ForEach(y => y
          .Must(x => Paths.Any(cur => string.Equals(cur, x.path, StringComparison.OrdinalIgnoreCase)))
          .WithMessage(
            $"Document contains invalid path. Only such paths are allowed: {Paths.Aggregate((x, y) => x + ", " + y)}")
          .Must(x => x.path == $"/{nameof(EditProjectRequest.Name)}" && projectRepository.DoesProjectNameExist(x.value.ToString()))
          .WithMessage(x => $"Project already exists."))
        .DependentRules(() =>
        {
          When(x => GetOperationByPath(x, Name) != null, () =>
            {
              RuleFor(x => x.Operations).UniqueOperationWithAllowedOp(Name, "replace");

              RuleFor(x => (string)GetOperationByPath(x, Name).value)
                .NotEmpty()
                .MaximumLength(150)
                .WithMessage("First name is too long.");
            });

          When(x => GetOperationByPath(x, ShortName) != null, () =>
            {
              RuleFor(x => x.Operations).UniqueOperationWithAllowedOp(ShortName, "replace");

              RuleFor(x => (string)GetOperationByPath(x, ShortName).value)
                .NotEmpty()
                .MaximumLength(30)
                .WithMessage("Short name is too long.");
            });

          When(x => GetOperationByPath(x, Description) != null, () =>
            {
              RuleFor(x => x.Operations).UniqueOperationWithAllowedOp(Description, "replace");
            });

          When(x => GetOperationByPath(x, ShortDescription) != null, () =>
            {
              RuleFor(x => x.Operations).UniqueOperationWithAllowedOp(ShortDescription, "replace");

              RuleFor(x => (string)GetOperationByPath(x, ShortDescription).value)
                .MaximumLength(300)
                .WithMessage("Short description is to long");
            });

          When(x => GetOperationByPath(x, Status) != null, () =>
            {
              RuleFor(x => x.Operations).UniqueOperationWithAllowedOp(Status, "replace");

              RuleFor(x => (ProjectStatusType)GetOperationByPath(x, Status).value)
                .IsInEnum()
                .WithMessage("Wrong status value.");
            });

          When(x => GetOperationByPath(x, DepartmentId) != null, () =>
            {
              RuleFor(x => x.Operations).UniqueOperationWithAllowedOp(DepartmentId, "replace");

              RuleFor(x => GetOperationByPath(x, DepartmentId).value)
                .Must(x => Guid.TryParse(x.ToString(), out Guid _))
                .WithMessage("Wrong department id value.")
                .Must(x => GetDepartment(Guid.Parse(DepartmentId)))
                .WithMessage("Project department does not found.");
            });
        });
    }

    private bool GetDepartment(Guid? departmentId)
    {
      if (!departmentId.HasValue)
      {
        return true;
      }

      try
      {
        Response<IOperationResult<IGetDepartmentsResponse>> response = _rcGetDepartments.GetResponse<IOperationResult<IGetDepartmentsResponse>>(
        IGetDepartmentsRequest.CreateObj(new() { departmentId.Value })).Result;

        if (response.Message.IsSuccess && response.Message.Body.Departments.FirstOrDefault() != null)
        {
          return true;
        }

        _logger.LogWarning(
          "Can not find department with this id {departmentId}: Errors: { Errors}",
          departmentId,
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Cannot edit project. Please try again later.");
      }

      return false;
    }
  }
}
