using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    class WorkersProjectIdsValidatorTests
    {
        private List<Guid> workerIds;

        private IValidator<WorkersIdsInProjectRequest> validator;
        private WorkersIdsInProjectRequest workersProjectRequest;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            validator = new WorkersProjectIdsValidator();
            workerIds = new List<Guid>();

            workersProjectRequest = new WorkersIdsInProjectRequest
            {
                ProjectId = new Guid(),
                WorkersIds = workerIds
            };
        }

        [Test]
        public void ShouldNotHaveValidationErrorsWhenRequestIsValid()
        {
            workerIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            workersProjectRequest.WorkersIds = workerIds;
            validator.TestValidate(workersProjectRequest).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldHaveValidationErrorWhenProjectIdEmpty()
        {
            workersProjectRequest.ProjectId = Guid.Empty;

            validator.TestValidate(workersProjectRequest).ShouldHaveValidationErrorFor(r => r.ProjectId);
        }

        [Test]
        public void ShouldHaveValidationErrorWhenWorkerIdEmpty()
        {
            workerIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.Empty
            };

            workersProjectRequest.ProjectId = Guid.NewGuid();
            workersProjectRequest.WorkersIds = workerIds;

            validator.TestValidate(workersProjectRequest).ShouldHaveValidationErrorFor(r => r.WorkersIds);
        }
    }
}
