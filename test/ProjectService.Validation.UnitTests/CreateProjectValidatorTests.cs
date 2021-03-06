﻿using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    class CreateProjectValidatorTests
    {
        private ICreateProjectValidator _validator;

        private ProjectRequest _newProject;

        [SetUp]
        public void SetUp()
        {
            _validator = new CreateProjectValidator();

            _newProject = new ProjectRequest
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
                        Role = UserRoleType.ProjectAdmin
                    }
                }
            };
        }

        [Test]
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
        public void ShouldErrorWhenProjectShortNameIsEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.ShortName, "");
        }

        [Test]
        public void ShouldErrorsWhenProjectShortNameIsEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.ShortName, "");
        }

        [Test]
        public void ShouldErrorWhenDescriptionIsEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.Description, "");
        }

        [Test]
        public void ShouldErrorWhenShortDescriptionIsEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.ShortDescription, "");
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
            var newProject = new ProjectRequest
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
                        Role = UserRoleType.ProjectAdmin
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
            var newProject = new ProjectRequest
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
                        Role = (UserRoleType)9
                    }
                }
            };

            _validator.TestValidate(newProject).ShouldHaveValidationErrorFor("Users[0].Role");
        }

        [Test]
        public void ShouldNotErrorsWhenRequestIsValid()
        {
            _validator.TestValidate(_newProject).ShouldNotHaveAnyValidationErrors();
        }
    }
}