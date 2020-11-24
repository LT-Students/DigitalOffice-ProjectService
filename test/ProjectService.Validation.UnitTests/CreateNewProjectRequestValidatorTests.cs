﻿using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    public class CreateNewProjectRequestValidatorTests
    {
        private IValidator<ProjectRequest> validator;
        private ProjectRequest projectRequest;

        [SetUp]
        public void SetUp()
        {
            validator = new ProjectValidator();

            projectRequest = new ProjectRequest
            {
                Project = new Project
                {
                    DepartmentId = Guid.NewGuid(),
                    ShortName = "DO",
                    Description = "New project for Lanit-Tercom",
                    IsActive = true,
                    Name = "12DigitalOffice24322525"
                },
                Users = new List<ProjectUser>
                {
                    new ProjectUser
                    {
                        User = new User
                        {
                            Id = Guid.NewGuid()
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
            List<ProjectUser> projectUser = null;

            validator.ShouldNotHaveValidationErrorFor(x => x.Users, projectUser);
        }

        [Test]
        public void ShouldNotHaveAnyValidationErrorsWhenRequestIsValid()
        {
            projectRequest.Project.Name = "12DigitalOffice24322525";

            validator.TestValidate(projectRequest).ShouldNotHaveAnyValidationErrors();
        }
    }
}