using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    public class EditProjectRequestValidatorTests
    {
        private IValidator<EditProjectRequest> validator;
        private EditProjectRequest editRequest;
        private IContractResolver resolver;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            validator = new EditProjectValidator();
            resolver = new CamelCasePropertyNamesContractResolver();
        }

        [SetUp]
        public void SetUp()
        {
            editRequest = new EditProjectRequest
            {
                Patch = new JsonPatchDocument<DbProject>(new List<Operation<DbProject>>
                {
                    new Operation<DbProject>("replace", "/Name", "", "New Project Name"),
                    new Operation<DbProject>("replace", "/ShortName", "", "NPN"),
                    new Operation<DbProject>("replace", "/Description", "", "New project description"),
                }, resolver),
                ProjectId = Guid.NewGuid()
            };
        }

        [Test]
        public void ShouldValidateEditProjectRequestWhenRequestIsCorrect()
        {
            validator.TestValidate(editRequest).ShouldNotHaveAnyValidationErrors();
        }

        #region Base validation
        [Test]
        public void ShouldThrowValidationExceptionWhenRequestNotContainsOperations()
        {
            editRequest.Patch.Operations.Clear();

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRequestContainsNotUniqueOperations()
        {
            editRequest.Patch.Operations.Add(editRequest.Patch.Operations.First());

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRequestContainsNotSupportedReplace()
        {
            editRequest.Patch.Operations.Add(new Operation<DbProject>("replace", "/Id", "", Guid.NewGuid().ToString()));

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }
        #endregion

        #region field validations
        [Test]
        public void ShouldValidateEditProjectRequestWhenNameIsValid()
        {
            editRequest.Patch.Operations.Find(x => x.path == "/Name").op = "add";
            validator.TestValidate(editRequest).ShouldNotHaveAnyValidationErrors();

            editRequest.Patch.Operations.Find(x => x.path == "/Name").op = "replace";
            validator.TestValidate(editRequest).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenNameIsTooLong()
        {
            editRequest.Patch.Operations.Find(x => x.path == "/Name").value = "".PadLeft(81);

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldValidateEditProjectRequestWhenShortNameIsValid()
        {
            editRequest.Patch.Operations.Find(x => x.path == "/ShortName").op = "add";
            validator.TestValidate(editRequest).ShouldNotHaveAnyValidationErrors();

            editRequest.Patch.Operations.Find(x => x.path == "/ShortName").op = "replace";
            validator.TestValidate(editRequest).ShouldNotHaveAnyValidationErrors();

            editRequest.Patch.Operations.Find(x => x.path == "/ShortName").op = "remove";
            validator.TestValidate(editRequest).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenShortNameIsTooLong()
        {
            editRequest.Patch.Operations.Find(x => x.path == "/ShortName").value = "".PadLeft(33);

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldValidateEditProjectRequestWhenDescriptionIsValid()
        {
            editRequest.Patch.Operations.Find(x => x.path == "/Description").op = "add";
            validator.TestValidate(editRequest).ShouldNotHaveAnyValidationErrors();

            editRequest.Patch.Operations.Find(x => x.path == "/Description").op = "replace";
            validator.TestValidate(editRequest).ShouldNotHaveAnyValidationErrors();

            editRequest.Patch.Operations.Find(x => x.path == "/Description").op = "remove";
            validator.TestValidate(editRequest).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenDescriptionIsTooLong()
        {
            editRequest.Patch.Operations.Find(x => x.path == "/Description").value = "".PadLeft(501);

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }
        #endregion
    }
}