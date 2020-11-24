using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    class WorkersProjectIdsValidatorTests
    {
        private IValidator<ProjectUser> validator;
        private ProjectUser workersProjectRequest;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            validator = new WorkersProjectValidator();

            workersProjectRequest = new ProjectUser
            {
                User = new User
                {
                    Id = Guid.NewGuid()
                }
            };
        }

        [Test]
        public void ShouldNotHaveValidationErrorsWhenRequestIsValid()
        {
            var workerId = Guid.NewGuid();

            workersProjectRequest.User.Id = workerId;
            validator.TestValidate(workersProjectRequest).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldHaveValidationErrorWhenWorkerIdEmpty()
        {
            var workerId = Guid.Empty;

            workersProjectRequest.User.Id = workerId;

            validator.TestValidate(workersProjectRequest).ShouldHaveValidationErrorFor(r => r.User.Id);
        }
    }
}
