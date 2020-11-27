using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    class WorkersProjectIdsValidatorTests
    {
        private IValidator<ProjectUserRequest> validator;
        private ProjectUserRequest userProjectRequest;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            validator = new ProjectUserRequestValidator();

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
