using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    public class EditProjectUserRequestValidatorTests
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

        Func<string, Operation> GetOperationByPath =>
            (path) => editRequest.Patch.Operations.Find(x => x.path == path);

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
            SuccessTestsWithOperationsForPath(EditProjectValidator.NamePath, false);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenNameIsTooLong()
        {
            GetOperationByPath(EditProjectValidator.NamePath).value = "".PadLeft(81);

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldValidateEditProjectRequestWhenShortNameIsValid()
        {
            SuccessTestsWithOperationsForPath(EditProjectValidator.ShortNamePath, true);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenShortNameIsTooLong()
        {
            GetOperationByPath(EditProjectValidator.ShortNamePath).value = "".PadLeft(33);

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldValidateEditProjectRequestWhenDescriptionIsValid()
        {
            SuccessTestsWithOperationsForPath(EditProjectValidator.DescriptionPath, true);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenDescriptionIsTooLong()
        {
            GetOperationByPath(EditProjectValidator.DescriptionPath).value = "".PadLeft(501);

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }

        public void SuccessTestsWithOperationsForPath(string path, bool nullable)
        {
            GetOperationByPath(path).op = "add";
            validator.TestValidate(editRequest).ShouldNotHaveAnyValidationErrors();

            GetOperationByPath(path).op = "replace";
            validator.TestValidate(editRequest).ShouldNotHaveAnyValidationErrors();

            if (nullable)
            {
                GetOperationByPath(path).op = "remove";
                validator.TestValidate(editRequest).ShouldNotHaveAnyValidationErrors();
            }
        }
        #endregion
    }
}