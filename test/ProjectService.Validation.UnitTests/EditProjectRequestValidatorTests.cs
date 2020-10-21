using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    public class EditProjectRequestValidatorTests
    {
        private IValidator<EditProjectRequest> validator;
        private EditProjectRequest editRequest;

        [SetUp]
        public void SetUp()
        {
            validator = new EditProjectValidator();

            editRequest = new EditProjectRequest
            {
                Id = Guid.NewGuid(),
                Name = "Name",
                Description = "Description",
                IsActive = false,
                DepartmentId = Guid.NewGuid()
            };
        }

        [Test]
        public void ShouldValidateEditProjectRequest()
        {
            validator.TestValidate(editRequest).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenDescriptionIsTooLong()
        {
            editRequest.Description = editRequest.Description.PadLeft(501);

            validator.TestValidate(editRequest).ShouldHaveValidationErrorFor(er => er.Description);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenNameIsNull()
        {
            editRequest.Name = null;

            validator.TestValidate(editRequest).ShouldHaveValidationErrorFor(er => er.Name);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenNameIsTooLong()
        {
            editRequest.Name = editRequest.Description.PadLeft(81);

            validator.TestValidate(editRequest).ShouldHaveValidationErrorFor(er => er.Name);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenGuidIsEmpty()
        {
            editRequest.Id = Guid.Empty;

            validator.TestValidate(editRequest).ShouldHaveValidationErrorFor(er => er.Id);
        }
    }
}