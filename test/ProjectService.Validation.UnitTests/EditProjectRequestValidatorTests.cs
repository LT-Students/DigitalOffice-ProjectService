using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
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
        private Mock<IProjectRepository> mockRepository;
        private EditProjectRequest editRequest;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            mockRepository = new Mock<IProjectRepository>();

            validator = new EditProjectValidator(mockRepository.Object);
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
                }, new CamelCasePropertyNamesContractResolver()),
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
        public void ShouldThrowValidationExceptionWhenNameIsTooLong()
        {
            editRequest.Patch.Operations.Find(x => x.path == "/Name").value = "".PadLeft(81);

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenShortNameIsTooLong()
        {
            editRequest.Patch.Operations.Find(x => x.path == "/ShortName").value = "".PadLeft(33);

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenDescriptionIsTooLong()
        {
            editRequest.Patch.Operations.Find(x => x.path == "/Description").value = "".PadLeft(501);

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenIsActiveAndClosedReasonIsNotValid1()
        {
            editRequest.Patch.Operations.Add(new Operation<DbProject>("replace", "/IsActive", "", true));
            editRequest.Patch.Operations.Add(new Operation<DbProject>("replace", "/ClosedReason", "", ProjectClosedReason.Completed));

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenIsActiveAndClosedReasonIsNotValid2()
        {
            editRequest.Patch.Operations.Add(new Operation<DbProject>("replace", "/IsActive", "", false));
            editRequest.Patch.Operations.Add(new Operation<DbProject>("replace", "/ClosedReason", "",(ProjectClosedReason)10000));

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenIsActiveAndClosedReasonIsNotValid3() //CastClosedReasonIfPossible
        {
            editRequest.Patch.Operations.Add(new Operation<DbProject>("replace", "/IsActive", "", false));
            editRequest.Patch.Operations.Add(new Operation<DbProject>("replace", "/ClosedReason", "", null));

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenIsActiveAndClosedReasonIsNotValid4()
        {
            editRequest.Patch.Operations.Add(new Operation<DbProject>("replace", "/ClosedReason", "", ProjectClosedReason.Failed));

            mockRepository.Setup(x => x.GetProject(editRequest.ProjectId)).Returns(new DbProject { IsActive = true });

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldValidateEditProjectRequestWhenIsActiveAndClosedReasonIsCorrect1()
        {
            editRequest.Patch.Operations.Add(new Operation<DbProject>("replace", "/IsActive", "", false));
            editRequest.Patch.Operations.Add(new Operation<DbProject>("replace", "/ClosedReason", "", ProjectClosedReason.Completed));

            validator.TestValidate(editRequest).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldValidateEditProjectRequestWhenIsActiveAndClosedReasonIsCorrect2()
        {
            editRequest.Patch.Operations.Add(new Operation<DbProject>("replace", "/ClosedReason", "", ProjectClosedReason.Failed));

            mockRepository.Setup(x => x.GetProject(editRequest.ProjectId)).Returns(new DbProject { IsActive = false });

            validator.TestValidate(editRequest).ShouldHaveAnyValidationError();
        }
        #endregion
    }
}