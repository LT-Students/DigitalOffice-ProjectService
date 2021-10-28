using System;
using System.Collections.Generic;
using FluentValidation.TestHelper;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Validation.Project;
using LT.DigitalOffice.ProjectService.Validation.Project.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
  class CreateProjectValidatorTests
  {
    //private ICreateProjectRequestValidator _validator;
    //private CreateProjectRequest _newProject;
    //private AutoMocker _autoMock;

    //[SetUp]
    //public void SetUp()
    //{
    //  _autoMock = new();

    //  _validator = new CreateProjectRequestValidator(
    //  _autoMock.CreateInstance<IProjectRepository>(),
    //  _autoMock.CreateInstance<ILogger<CreateProjectRequestValidator>>(),
    //  _autoMock.CreateInstance<IRequestClient<ICheckDepartmentsExistence>>(),
    //  _autoMock.CreateInstance<IRequestClient<ICheckUsersExistence>>(),
    //  _autoMock.CreateInstance<IImageValidator>());

    //  _newProject = new CreateProjectRequest
    //  {
    //    Name = "Project for Lanit-Tercom",
    //    ShortName = "Project",
    //    Description = "New project for Lanit-Tercom",
    //    ShortDescription = "Short description",
    //    DepartmentId = Guid.NewGuid(),
    //    Status = ProjectStatusType.Active,
    //    Users = new List<CreateUserRequest>
    //            {
    //                new CreateUserRequest
    //                {
    //                    UserId = Guid.NewGuid(),
    //                    Role = ProjectUserRoleType.Manager
    //                }
    //            }
    //  };
    //}

    /*[Test]
    public void ShouldErrorWhenProjectNameIsEmpty()
    {
      _validator.ShouldHaveValidationErrorFor(x => x.Name, "");
    }

    [Test]
    public void ShouldErrorWhenProjectShortNameIsTooLong()
    {
      var shortName = "Short name".PadLeft(100);

      _validator.ShouldHaveValidationErrorFor(x => x.ShortName, shortName);
    }

    [Test]
    public void ShouldNotErrorWhenProjectShortNameIsEmpty()
    {
      _validator.ShouldNotHaveValidationErrorFor(x => x.ShortName, "");
    }

    [Test]
    public void ShouldNotErrorsWhenProjectShortNameIsEmpty()
    {
      _validator.ShouldNotHaveValidationErrorFor(x => x.ShortName, "");
    }

    [Test]
    public void ShouldNotErrorWhenDescriptionIsEmpty()
    {
      _validator.ShouldNotHaveValidationErrorFor(x => x.Description, "");
    }

    [Test]
    public void ShouldNotErrorWhenShortDescriptionIsEmpty()
    {
      _validator.ShouldNotHaveValidationErrorFor(x => x.ShortDescription, "");
    }

    [Test]
    public void ShouldNotErrorsWhenShortDescriptionIsTooLong()
    {
      var shortDescription = "Short description".PadLeft(300);

      _validator.ShouldNotHaveValidationErrorFor(x => x.ShortDescription, shortDescription);
    }

    [Test]
    public void ShouldErrorForWhenUserIdIsEmpty()
    {
      var newProject = new CreateProjectRequest
      {
        Name = "Project for Lanit-Tercom",
        ShortName = "Project",
        Description = "New project for Lanit-Tercom",
        ShortDescription = "Short description",
        DepartmentId = Guid.NewGuid(),
        Status = ProjectStatusType.Active,
        Users = new List<ProjectUserRequest>
                {
                    new ProjectUserRequest
                    {
                        UserId = Guid.Empty,
                        Role = ProjectUserRoleType.Manager
                    }
                }
      };

      _validator.TestValidate(newProject).ShouldHaveValidationErrorFor("Users[0].UserId");
    }

    [Test]
    public void ShouldErrorForWhenUserStatusIsOutEnum()
    {
      _validator.ShouldHaveValidationErrorFor(x => x.Status, (ProjectStatusType)10);
    }

    [Test]
    public void ShouldErrorForWhenUserOutEnum()
    {
      var newProject = new CreateProjectRequest
      {
        Name = "Project for Lanit-Tercom",
        ShortName = "Project",
        Description = "New project for Lanit-Tercom",
        ShortDescription = "Short description",
        DepartmentId = Guid.NewGuid(),
        Status = ProjectStatusType.Active,
        Users = new List<ProjectUserRequest>
                {
                    new ProjectUserRequest
                    {
                        UserId = Guid.NewGuid(),
                        Role = (ProjectUserRoleType)9
                    }
                }
      };

      _validator.TestValidate(newProject).ShouldHaveValidationErrorFor("Users[0].Role");
    }

    [Test]
    public void ShouldNotErrorsWhenRequestIsValid()
    {
      _validator.TestValidate(_newProject).ShouldNotHaveAnyValidationErrors();
    }*/
  }
}
