using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Commands
{
    public class EditProjectByIdCommandTests
    {
        private DbProject dbProject;
        private EditProjectRequest editRequest;
        private IEditProjectByIdCommand command;
        private Mock<IProjectRepository> repositoryMock;
        private Mock<IValidator<EditProjectRequest>> validatorMock;
        private Mock<IAccessValidator> accessValidatorMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            editRequest = new EditProjectRequest
            {
                ProjectId = Guid.NewGuid(),
                Patch = new JsonPatchDocument<DbProject>()
            };

            dbProject = new DbProject
            {
                Id = editRequest.ProjectId
            };
        }

        [SetUp]
        public void SetUp()
        {
            validatorMock = new Mock<IValidator<EditProjectRequest>>();
            repositoryMock = new Mock<IProjectRepository>();
            accessValidatorMock = new Mock<IAccessValidator>();

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(true);

            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(true);

            repositoryMock
                .Setup(x => x.GetProject(dbProject.Id))
                .Returns(dbProject);

            repositoryMock
                .Setup(r => r.EditProjectById(dbProject))
                .Returns(editRequest.ProjectId);

            command = new EditProjectByIdCommand(validatorMock.Object, repositoryMock.Object, accessValidatorMock.Object);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenEditProjectRequestIsInvalid()
        {
            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => command.Execute(editRequest));

            repositoryMock.Verify(r => r.EditProjectById(dbProject), Times.Never);
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenUserIsNotAdminAndHasNoRights()
        {
            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            accessValidatorMock
               .Setup(x => x.IsAdmin())
               .Returns(false);

            Assert.Throws<ForbiddenException>(
                () => command.Execute(editRequest), "Project with this ID has been found");
            validatorMock.Verify(v => v.Validate(It.IsAny<IValidationContext>()), Times.Once);
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenDbProjectWasNotFound()
        {
            repositoryMock
                .Setup(x => x.EditProjectById(It.IsAny<DbProject>()))
                .Throws(new NullReferenceException());

            Assert.Throws<NullReferenceException>(
                () => command.Execute(editRequest), "Project with this ID has been found");
            validatorMock.Verify(v => v.Validate(It.IsAny<IValidationContext>()), Times.Once);
        }

        [Test]
        public void ShouldReturnProjectGuidWhenEverythingIsRight()
        {
            Assert.AreEqual(editRequest.ProjectId, command.Execute(editRequest));
            validatorMock.Verify(v => v.Validate(It.IsAny<IValidationContext>()), Times.Once);
            repositoryMock.Verify(r => r.GetProject(dbProject.Id), Times.Once);
            repositoryMock.Verify(r => r.EditProjectById(dbProject), Times.Once);
        }

        [Test]
        public void ShouldReturnProjectGuidWhenUserIsOnlyAdmin()
        {
            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            Assert.AreEqual(editRequest.ProjectId, command.Execute(editRequest));
            validatorMock.Verify(v => v.Validate(It.IsAny<IValidationContext>()), Times.Once);
            repositoryMock.Verify(r => r.GetProject(dbProject.Id), Times.Once);
            repositoryMock.Verify(r => r.EditProjectById(dbProject), Times.Once);
        }

        [Test]
        public void ShouldReturnProjectGuidWhenUserIsOnlyHasRights()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(false);

            Assert.AreEqual(editRequest.ProjectId, command.Execute(editRequest));
            validatorMock.Verify(v => v.Validate(It.IsAny<IValidationContext>()), Times.Once);
            repositoryMock.Verify(r => r.EditProjectById(dbProject), Times.Once);
        }
    }
}