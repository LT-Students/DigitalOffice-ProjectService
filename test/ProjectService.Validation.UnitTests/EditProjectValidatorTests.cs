using System;
using System.Collections.Generic;
using FluentValidation;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using Moq.AutoMock;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
  internal class EditProjectValidatorTests
  {
    private IValidator<JsonPatchDocument<EditProjectRequest>> _validator;
    private JsonPatchDocument<EditProjectRequest> _request;
    private AutoMocker _autoMock;

    Func<string, Operation> GetOperationByPath =>
        (path) => _request.Operations.Find(x => x.path == path);

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _autoMock = new();

      _validator = new EditProjectValidator(
        _autoMock.CreateInstance<IProjectRepository>(),
        _autoMock.CreateInstance<ILogger<EditProjectValidator>>(),
        _autoMock.CreateInstance<IRequestClient<IGetDepartmentsRequest>>());
    }

    [SetUp]
    public void SetUp()
    {
      _request = new JsonPatchDocument<EditProjectRequest>(new List<Operation<EditProjectRequest>>
            {
                new Operation<EditProjectRequest>(
                    "replace",
                    $"/{nameof(EditProjectRequest.Name)}",
                    "",
                    "value"),

                new Operation<EditProjectRequest>(
                    "replace",
                    $"/{nameof(EditProjectRequest.ShortName)}",
                    "",
                    "value"),

                new Operation<EditProjectRequest>(
                    "replace",
                    $"/{nameof(EditProjectRequest.Description)}",
                    "",
                    "value"),

                new Operation<EditProjectRequest>(
                    "replace",
                    $"/{nameof(EditProjectRequest.ShortDescription)}",
                    "",
                    "value"),

                new Operation<EditProjectRequest>(
                    "replace",
                    $"/{nameof(EditProjectRequest.Status)}",
                    "",
                    ProjectStatusType.Active),

                new Operation<EditProjectRequest>(
                    "replace",
                    $"/{nameof(EditProjectRequest.DepartmentId)}",
                    "",
                    Guid.NewGuid())

            }, new CamelCasePropertyNamesContractResolver());
    }

    /*[Test]
    public void SuccessValidation()
    {
      _validator.TestValidate(_request).ShouldNotHaveAnyValidationErrors();
    }

    #region Base validate errors

    [Test]
    public void ExceptionWhenRequestNotContainsOperations()
    {
      _request.Operations.Clear();

      _validator.TestValidate(_request).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ExceptionWhenRequestContainsNotUniqueOperations()
    {
      _request.Operations.Add(_request.Operations.First());

      _validator.TestValidate(_request).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ExceptionWhenRequestContainsNotSupportedReplace()
    {
      _request.Operations.Add(new Operation<EditProjectRequest>("replace", $"/{nameof(DbProject.Id)}", "", Guid.NewGuid()));

      _validator.TestValidate(_request).ShouldHaveAnyValidationError();
    }
    #endregion

    #region Names size checks
    [Test]
    public void ExceptionWhenNameIsTooLong()
    {
      GetOperationByPath(EditProjectValidator.Name).value = "".PadLeft(151);

      _validator.TestValidate(_request).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ExceptionWhenShortNameIsTooLong()
    {
      GetOperationByPath(EditProjectValidator.ShortName).value = "".PadLeft(31);

      _validator.TestValidate(_request).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ExceptionWhenShortDescriptionIsTooLong()
    {
      GetOperationByPath(EditProjectValidator.ShortDescription).value = "".PadLeft(301);

      _validator.TestValidate(_request).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ExceptionWhenStatusIsNotContainedInEnum()
    {
      GetOperationByPath(EditProjectValidator.Status).value = 10;

      _validator.TestValidate(_request).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ExceptionWhenDepartmentIdIsInvalid()
    {
      GetOperationByPath(EditProjectValidator.DepartmentId).value = "not guid";

      _validator.TestValidate(_request).ShouldHaveAnyValidationError();
    }
    #endregion*/
  }
}
