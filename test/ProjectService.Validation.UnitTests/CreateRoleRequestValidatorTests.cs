﻿using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    public class CreateRoleRequestValidatorTests
    {
        private IValidator<CreateRoleRequest> validator;
        private CreateRoleRequest roleRequest;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            roleRequest = new CreateRoleRequest
            {
                Name = "Lead Tester Grand Manager 2nd",
                Description = "New role in DigitalOffice"
            };
        }

        [SetUp]
        public void SetUp()
        {
            validator = new RoleValidator();
        }

        [Test]
        public void ShouldHaveValidationErrorWhenRoleNameIsEmpty()
        {
            validator.ShouldHaveValidationErrorFor(x => x.Name, "");
        }

        [Test]
        public void ShouldHaveValidationErrorForWhenRoleNameIsTooLong()
        {
            var name = roleRequest.Name.PadLeft(81);

            validator.ShouldHaveValidationErrorFor(er => er.Name, name);
        }

        [Test]
        public void ShouldHaveValidationErrorForWhenRoleDescriptionIsTooLong()
        {
            var description =  roleRequest.Description.PadLeft(612);

            validator.ShouldHaveValidationErrorFor(er => er.Description, description);
        }

        [Test]
        public void ShouldNotHaveAnyValidationErrorsWhenRequestIsValid()
        {
            validator.TestValidate(roleRequest).ShouldNotHaveAnyValidationErrors();
        }
    }
}