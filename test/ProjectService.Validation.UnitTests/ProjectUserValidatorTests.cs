﻿using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using NUnit.Framework;
using System;

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
            _validator.ShouldHaveValidationErrorFor(x => x.Role, (UserRoleType)10000);

        }

        [Test]
        public void ShouldNotErrorsWhenRequestIsValid()
        {
            var projectUserRequest = new ProjectUserRequest
            {
                UserId = Guid.NewGuid(),
                Role = UserRoleType.ProjectAdmin
            };

            _validator.TestValidate(projectUserRequest).ShouldNotHaveAnyValidationErrors();
        }
    }
}