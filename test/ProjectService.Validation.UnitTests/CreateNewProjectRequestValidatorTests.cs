using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    public class CreateNewProjectRequestValidatorTests
    {
        private IValidator<NewProjectRequest> validator;
        private NewProjectRequest projectRequest;

        [SetUp]
        public void SetUp()
        {
            validator = new ProjectValidator();

            projectRequest = new NewProjectRequest
            {
                DepartmentId = Guid.NewGuid(),
                ShortName = "DO",
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
        public void ShouldHaveValidationErrorWhenProjectShortNameIsEmpty()
        {
            validator.ShouldHaveValidationErrorFor(x => x.Name, "");
        }

        [Test]
        public void ShouldHaveValidationErrorWhenProjectShortNameIsTooLong()
        {
            var shortName = projectRequest.Description.PadLeft(100);

            validator.ShouldHaveValidationErrorFor(x => x.ShortName, shortName);
        }

        [Test]
        public void ShouldNotHaveAnyValidationErrorsWhenProjectShortNameIsNull()
        {
            string shortName = null;

            validator.ShouldNotHaveValidationErrorFor(x => x.ShortName, shortName);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenDescriptionIsTooLong()
        {
            var description = projectRequest.Description.PadLeft(501);

            validator.ShouldHaveValidationErrorFor(er => er.Description, description);
        }

        [Test]
        public void ShouldNotHaveAnyValidationErrorsWhenDescriptionIsNull()
        {
            string description = null;

            validator.ShouldNotHaveValidationErrorFor(er => er.Description, description);
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