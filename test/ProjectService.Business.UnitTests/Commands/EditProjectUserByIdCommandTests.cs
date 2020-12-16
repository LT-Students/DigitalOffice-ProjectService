using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Commands
{
    public class EditProjectUserByIdCommandTests
    {
        private DbProjectUser dbProjectUser;
        private EditProjectUserRequest editRequest;
        private IEditProjectUserByIdCommand command;
        private Mock<IProjectRepository> repositoryMock;
        private Mock<IValidator<EditProjectUserRequest>> validatorMock;
        private Mock<IAccessValidator> accessValidatorMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            editRequest = new EditProjectUserRequest
            {
                ProjectUserId = Guid.NewGuid(),
                Patch = new JsonPatchDocument<DbProjectUser>()
            };

            dbProjectUser = new DbProjectUser
            {
                Id = editRequest.ProjectUserId
            };
        }

        [SetUp]
        public void SetUp()
        {
            validatorMock = new Mock<IValidator<EditProjectUserRequest>>();
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
                .Setup(x => x.GetProjectUserById(dbProjectUser.Id))
                .Returns(dbProjectUser);

            repositoryMock
                .Setup(r => r.EditProjectUserById(dbProjectUser))
                .Returns(editRequest.ProjectUserId);

            command = new EditProjectUserByIdCommand(repositoryMock.Object, validatorMock.Object, accessValidatorMock.Object);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenEditProjectUserRequestIsInvalid()
        {
            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => command.Execute(editRequest));

            accessValidatorMock.Verify(r => r.HasRights(It.IsAny<int>()), Times.Never);
            repositoryMock.Verify(r => r.GetProjectUserById(It.IsAny<Guid>()), Times.Never);
            repositoryMock.Verify(r => r.EditProjectUserById(dbProjectUser), Times.Never);
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

            Assert.Throws<ForbiddenException>(() => command.Execute(editRequest));
            repositoryMock.Verify(r => r.GetProjectUserById(It.IsAny<Guid>()), Times.Never);
            repositoryMock.Verify(r => r.EditProjectUserById(dbProjectUser), Times.Never);
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenDbProjectUserWasNotFound1()
        {
            repositoryMock
                .Setup(x => x.GetProject(It.IsAny<Guid>()))
                .Throws(new NullReferenceException());

            Assert.Throws<NullReferenceException>(() => command.Execute(editRequest));
            repositoryMock.Verify(r => r.EditProjectUserById(dbProjectUser), Times.Never);
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenDbProjectUserWasNotFound2()
        {
            repositoryMock
                .Setup(x => x.EditProjectById(It.IsAny<DbProject>()))
                .Throws(new NullReferenceException());

            Assert.Throws<NullReferenceException>(() => command.Execute(editRequest));
        }

        [Test]
        public void ShouldReturnProjectUserGuidWhenEverythingIsRight()
        {
            Assert.AreEqual(editRequest.ProjectUserId, command.Execute(editRequest));
            validatorMock.Verify(v => v.Validate(It.IsAny<IValidationContext>()), Times.Once);
            repositoryMock.Verify(r => r.GetProjectUserById(dbProjectUser.Id), Times.Once);
            repositoryMock.Verify(r => r.EditProjectUserById(dbProjectUser), Times.Once);
        }

        [Test]
        public void ShouldReturnProjectUserGuidWhenUserIsOnlyAdmin()
        {
            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            Assert.AreEqual(editRequest.ProjectUserId, command.Execute(editRequest));
        }

        [Test]
        public void ShouldReturnProjectUserGuidWhenUserIsOnlyHasRights()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(false);

            Assert.AreEqual(editRequest.ProjectUserId, command.Execute(editRequest));
        }
    }
}