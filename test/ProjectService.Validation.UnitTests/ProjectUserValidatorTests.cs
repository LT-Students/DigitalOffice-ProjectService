﻿using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    internal class ProjectUserValidatorTests
    {
        private IProjectUserValidator validator;
        private ProjectUserRequest userProjectRequest;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            validator = new ProjectUserValidator();
        }

        [SetUp]
        public void SetUp()
        {
            userProjectRequest = new ProjectUserRequest
            {
                RoleId = Guid.NewGuid(),
                User = new UserRequest
                {
                    Id = Guid.NewGuid()
                }
            };
        }

        [Test]
        public void ShouldNotHaveValidationErrorsWhenUserIsNull()
        {
            ProjectUserRequest newProjectUser = new ProjectUserRequest()
            {
                User = null
            };

            validator.TestValidate(newProjectUser).ShouldHaveValidationErrorFor(r => r.User);
        }

        [Test]
        public void ShouldNotHaveValidationErrorsWhenRequestIsValid()
        {
            validator.TestValidate(userProjectRequest).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldHaveValidationErrorWhenUserIdIsEmpty()
        {
            userProjectRequest.User.Id = Guid.Empty;

            validator.TestValidate(userProjectRequest).ShouldHaveValidationErrorFor(r => r.User.Id);
        }

        [Test]
        public void ShouldHaveValidationErrorWhenRoleIdIsEmpty()
        {
            userProjectRequest.RoleId = Guid.Empty;

            validator.TestValidate(userProjectRequest).ShouldHaveValidationErrorFor(r => r.RoleId);
        }
    }
}
