using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
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
    public class EditProjectUserByIdCommandTests
    {
        private Guid projectUserId;
        private DbProjectUser dbProjectUser;
        private EditProjectUserRequest editRequest;
        private IEditProjectUserByIdCommand command;
        private Mock<IProjectRepository> repositoryMock;
        private Mock<IValidator<EditProjectUserRequest>> validatorMock;
        private Mock<IAccessValidator> accessValidatorMock;

        [SetUp]
        public void SetUp()
        {
            projectUserId = Guid.NewGuid();

            validatorMock = new Mock<IValidator<EditProjectUserRequest>>();
            repositoryMock = new Mock<IProjectRepository>();
            accessValidatorMock = new Mock<IAccessValidator>();

            command = new EditProjectUserByIdCommand(repositoryMock.Object, validatorMock.Object, accessValidatorMock.Object);

            editRequest = new EditProjectUserRequest
            {
                Patch = new JsonPatchDocument<DbProjectUser>(null, new ContractResolver)
                Id = projectUserId,
                Name = "DigitalOffice",
                ShortName = "DO",
                DepartmentId = Guid.NewGuid(),
                Description = "A new description",
                IsActive = false
            };

            dbProjectUser = new DbProjectUser
            {
                Id = projectUserId,
                N = editRequest.Name,
                ShortName = editRequest.ShortName,
                DepartmentId = editRequest.DepartmentId,
                Description = editRequest.Description,
                IsActive = editRequest.IsActive
            };
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenEditProjectRequestIsInvalid()
        {
            validatorMock
                    .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                    .Returns(false);

            Assert.Throws<ValidationException>(
                () => command.Execute(projectUserId, editRequest), "Validator did not throw a ValidationException");

            mapperMock.Verify(m => m.Map(editRequest), Times.Never);
            repositoryMock.Verify(r => r.EditProjectById(dbProjectUser), Times.Never);
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenDbProjectWasNotFound()
        {
            validatorMock
                 .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                 .Returns(true);

            mapperMock
                .Setup(x => x.Map(It.IsAny<EditProjectUserRequest>()))
                .Returns(dbProjectUser);

            repositoryMock
                .Setup(x => x.EditProjectById(It.IsAny<DbProject>()))
                .Throws(new NullReferenceException());

            Assert.Throws<NullReferenceException>(
                () => command.Execute(projectUserId, editRequest), "Project with this ID has been found");
            validatorMock.Verify(v => v.Validate(It.IsAny<IValidationContext>()), Times.Once);
            mapperMock.Verify(m => m.Map(editRequest), Times.Once);
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenEditProjectRequestIsNull()
        {
            editRequest = null;

            validatorMock
                 .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                 .Returns(true);

            mapperMock
                .Setup(x => x.Map(It.IsAny<EditProjectUserRequest>()))
                .Returns(dbProjectUser);

            repositoryMock
                .Setup(x => x.EditProjectById(It.IsAny<DbProject>()))
                .Returns(projectUserId);

            Assert.Throws<ArgumentNullException>(
                () => command.Execute(projectUserId, editRequest), "Argument was not null");
            validatorMock.Verify(v => v.Validate(It.IsAny<IValidationContext>()), Times.Never);
            mapperMock.Verify(m => m.Map(It.IsAny<EditProjectUserRequest>()), Times.Never);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenProjectGuidIsEmpty()
        {
            validatorMock
                 .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                 .Returns(false);

            mapperMock
                .Setup(x => x.Map(It.IsAny<EditProjectUserRequest>()))
                .Returns(dbProjectUser);

            repositoryMock
                .Setup(x => x.EditProjectById(It.IsAny<DbProject>()))
                .Returns(projectUserId);

            Assert.Throws<ValidationException>(
                () => command.Execute(projectUserId, editRequest),
                "Argument was not null");
            validatorMock.Verify(v => v.Validate(It.IsAny<IValidationContext>()), Times.Once);
            mapperMock.Verify(m => m.Map(It.IsAny<EditProjectUserRequest>()), Times.Never);
            repositoryMock.Verify(r => r.EditProjectById(dbProjectUser), Times.Never);
        }

        [Test]
        public void ShouldReturnProjectGuidWhenProjectIsEdited()
        {
            validatorMock
                .Setup(v => v.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);
            mapperMock
                .Setup(m => m.Map(It.IsAny<EditProjectUserRequest>()))
                .Returns(dbProjectUser);
            repositoryMock
                .Setup(r => r.EditProjectById(dbProjectUser))
                .Returns(projectUserId);

            Assert.AreEqual(projectUserId, command.Execute(projectUserId, editRequest));
            validatorMock.Verify(v => v.Validate(It.IsAny<IValidationContext>()), Times.Once);
            repositoryMock.Verify(r => r.EditProjectById(dbProjectUser), Times.Once);
            mapperMock.Verify(m => m.Map(editRequest), Times.Once);
        }
    }
}