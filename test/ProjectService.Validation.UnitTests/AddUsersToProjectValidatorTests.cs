using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    public class AddUsersToProjectValidatorTests
    {
        private IValidator<AddUsersToProjectRequest> validator;
        private AddUsersToProjectRequest _request;

        [SetUp]
        public void SetUp()
        {
            validator = new AddUsersToProjectValidator();

            var users = new List<UserRequest>
            {
                new UserRequest
                {
                    Id = Guid.NewGuid(),
                    IsActive = true
                },
                new UserRequest
                {
                    Id = Guid.NewGuid(),
                    IsActive = true
                }
            };

            var projectUsers = new List<ProjectUserRequest>
            {
                new ProjectUserRequest
                {
                    RoleId = Guid.NewGuid(),
                    User = users.ElementAt(0)
                },
                new ProjectUserRequest
                {
                    RoleId = Guid.NewGuid(),
                    User = users.ElementAt(0)
                }
            };

            _request = new AddUsersToProjectRequest
            {
                ProjectId = Guid.NewGuid(),
                Users = projectUsers
            };
        }

        [Test]
        public void ShouldHaveValidationErrorWhenProjectIdIsEmpty()
        {
            var emptyProjectId = Guid.Empty;

            validator.ShouldHaveValidationErrorFor(x => x.ProjectId, emptyProjectId);
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
            validator.TestValidate(_request).ShouldNotHaveAnyValidationErrors();
        }
    }
}
