using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    internal class CreateNewProjectRequestValidatorTests
    {
        private IValidator<ProjectExpandedRequest> validator;
        private ProjectExpandedRequest projectRequest;

        [SetUp]
        public void SetUp()
        {
            validator = new ProjectExpandedRequestValidator();

            projectRequest = new ProjectExpandedRequest
            {
                Project = new Project
                {
                    DepartmentId = Guid.NewGuid(),
                    ShortName = "DO",
                    Description = "New project for Lanit-Tercom",
                    IsActive = true,
                    Name = "12DigitalOffice24322525",
                },
                Users = new List<ProjectUserRequest>
                {
                    new ProjectUserRequest
                    {
                        RoleId = Guid.NewGuid(),
                        User = new UserRequest
                        {
                            Id = Guid.NewGuid(),
                            IsActive = true
                        }
                    }
                }
            };
        }

        [Test]
        public void ShouldHaveValidationErrorWhenProjectNameIsEmpty()
        {
            validator.ShouldHaveValidationErrorFor(x => x.Project.Name, "");
        }

        [Test]
        public void ShouldHaveValidationErrorWhenProjectShortNameIsTooLong()
        {
            var shortName = projectRequest.Project.Description.PadLeft(100);

            validator.ShouldHaveValidationErrorFor(x => x.Project.ShortName, shortName);
        }

        [Test]
        public void ShouldNotHaveAnyValidationErrorsWhenProjectShortNameIsNull()
        {
            string shortName = null;

            validator.ShouldNotHaveValidationErrorFor(x => x.Project.ShortName, shortName);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenDescriptionIsTooLong()
        {
            var description = projectRequest.Project.Description.PadLeft(501);

            validator.ShouldHaveValidationErrorFor(x => x.Project.Description, description);
        }

        [Test]
        public void ShouldNotHaveAnyValidationErrorsWhenDescriptionIsNull()
        {
            string description = null;

            validator.ShouldNotHaveValidationErrorFor(x => x.Project.Description, description);
        }

        [Test]
        public void ShouldHaveValidationErrorForWhenProjectNameIsTooLong()
        {
            projectRequest.Project.Name += projectRequest.Project.Name.PadLeft(81);

            validator.TestValidate(projectRequest).ShouldHaveValidationErrorFor(request => request.Project.Name)
                .WithErrorMessage("Project name is too long.");
        }

        [Test]
        public void ShouldHaveValidationErrorForWhenUserIsNull()
        {
            List<ProjectUserRequest> projectUser = null;

            validator.ShouldNotHaveValidationErrorFor(x => x.Users, projectUser);
        }

        [Test]
        public void ShouldNotHaveAnyValidationErrorsWhenRequestIsValid()
        {
            projectRequest.Project.Name = "12DigitalOffice24322525";

            validator.TestValidate(projectRequest).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldHaveValidationErrorWhenRoleIdIsNull()
        {
            projectRequest.Users.First().RoleId = Guid.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.Users, projectRequest.Users);
        }
    }
}