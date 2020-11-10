using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    public class CreateRoleRequestValidatorTests
    {
        private IValidator<CreateRoleRequest> validator;
        private CreateRoleRequest roleRequest;

        [SetUp]
        public void SetUp()
        {
            validator = new RoleValidator();

            roleRequest = new CreateRoleRequest
            {
                Name = "Lead Tester Grand Manager 2nd",
                Description = "New role in DigitalOffice"
            };
        }

        [Test]
        public void ShouldHaveValidationErrorWhenRoleNameIsEmpty()
        {
            validator.ShouldHaveValidationErrorFor(x => x.Name, "");
        }

        [Test]
        public void ShouldHaveValidationErrorForWhenRoleNameIsTooLong()
        {
            roleRequest.Name += roleRequest.Name.PadLeft(81);

            validator.TestValidate(roleRequest).ShouldHaveValidationErrorFor(request => request.Name)
                .WithErrorMessage("Role name is too long.");
        }

        [Test]
        public void ShouldHaveValidationErrorForWhenRoleDescriptionIsTooLong()
        {
            roleRequest.Description += roleRequest.Description.PadLeft(612);

            validator.TestValidate(roleRequest).ShouldHaveValidationErrorFor(request => request.Description)
                .WithErrorMessage("Role description is too long.");
        }

        [Test]
        public void ShouldNotHaveAnyValidationErrorsWhenRequestIsValid()
        {
            validator.TestValidate(roleRequest).ShouldNotHaveAnyValidationErrors();
        }
    }
}