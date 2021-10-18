using System;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.User;
using LT.DigitalOffice.ProjectService.Validation.User.Interfaces;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
  class ProjectUserValidatorTests
  {
    private IProjectUserValidator _validator;

    [SetUp]
    public void SetUp()
    {
      _validator = new ProjectUserValidator();
    }

    [Test]
    public void ShouldThrowExceptionWhenUserIdIsEmpty()
    {
      _validator.ShouldHaveValidationErrorFor(x => x.UserId, Guid.Empty);
    }

    [Test]
    public void ShouldThrowExceptionWhenRoleIsBad()
    {
      _validator.ShouldHaveValidationErrorFor(x => x.Role, (ProjectUserRoleType)10000);

    }

    [Test]
    public void ShouldNotErrorsWhenRequestIsValid()
    {
      var projectUserRequest = new CreateUserRequest
      {
        UserId = Guid.NewGuid(),
        Role = ProjectUserRoleType.Manager
      };

      _validator.TestValidate(projectUserRequest).ShouldNotHaveAnyValidationErrors();
    }
  }
}
