using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestModels;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    class WorkersProjectIdsValidatorTests
    {
        private IValidator<ProjectUserRequest> validator;
        private ProjectUserRequest workersProjectRequest;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            validator = new WorkersProjectValidator();

            workersProjectRequest = new ProjectUserRequest
            {
                User = new UserRequest
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
