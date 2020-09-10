using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto;
using LT.DigitalOffice.ProjectService.Validation;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Validators
{
    public class CreateNewProjectRequestValidatorTests
    {
        private IValidator<NewProjectRequest> validator;
        private NewProjectRequest projectRequest;

        [SetUp]
        public void SetUp()
        {
            validator = new NewProjectValidator();

            projectRequest = new NewProjectRequest
            {
                DepartmentId = Guid.NewGuid(),
                Description = "New project for Lanit-Tercom",
                IsActive = true,
                Name = "12DigitalOffice24322525"
            };
        }

        [Test]
        public void ShouldHaveValidationErrorWhenProjectNameIsEmpty()
        {
            validator.ShouldHaveValidationErrorFor(x => x.Name, "");
        }

        [Test]
        public void ShouldHaveValidationErrorForWhenProjectNameIsTooLong()
        {
            projectRequest.Name += projectRequest.Name.PadLeft(81);

            validator.TestValidate(projectRequest).ShouldHaveValidationErrorFor(request => request.Name)
                .WithErrorMessage("Project name is too long.");
        }

        [Test]
        public void ShouldNotHaveAnyValidationErrorsWhenRequestIsValid()
        {
            validator.TestValidate(projectRequest).ShouldNotHaveAnyValidationErrors();
        }
    }
}