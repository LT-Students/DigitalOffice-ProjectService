using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Broker.UnitTests.Commands
{
    internal class CreateNewProjectCommandTests
    {
        private ICreateNewProjectCommand command;

        private Mock<IProjectRepository> repositoryMock;
        private Mock<IValidator<ProjectExpandedRequest>> validatorMock;
        private Mock<IProjectExpandedRequestMapper> mapperMock;
        private Mock<IAccessValidator> accessValidator;

        private DbProject newProject;
        private ProjectExpandedRequest newRequest;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            newRequest = new ProjectExpandedRequest
            {
                Project = new ProjectRequest(),
                Users = new List<ProjectUserRequest>()
            };

            newProject = new DbProject
            {
                Id = Guid.NewGuid()
            };
        }

        [SetUp]
        public void SetUp()
        {
            validatorMock = new Mock<IValidator<ProjectExpandedRequest>>();
            repositoryMock = new Mock<IProjectRepository>();
            mapperMock = new Mock<IProjectExpandedRequestMapper>();
            accessValidator = new Mock<IAccessValidator>();

            command = new CreateNewProjectCommand(repositoryMock.Object, validatorMock.Object, mapperMock.Object, accessValidator.Object);

            validatorMock
                 .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                 .Returns(true)
                 .Verifiable();

            mapperMock
                .Setup(x => x.Map(It.IsAny<ProjectExpandedRequest>()))
                .Returns(newProject)
                .Verifiable();

            repositoryMock
                .Setup(x => x.CreateNewProject(It.IsAny<DbProject>()))
                .Returns(newProject.Id)
                .Verifiable();

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(true)
                .Verifiable();

            accessValidator
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(true)
                .Verifiable();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenCreatingNewProjectWithIncorrectProjectData()
        {
            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => command.Execute(newRequest), "Project field validation error");
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            mapperMock.Verify(mapper => mapper.Map(It.IsAny<ProjectExpandedRequest>()), Times.Never);
            repositoryMock.Verify(repository => repository.CreateNewProject(It.IsAny<DbProject>()), Times.Never);
        }

        [Test]
        public void ShouldThrowsExceptionWhenRepositoryThrowsException()
        {
            repositoryMock
                .Setup(x => x.CreateNewProject(It.IsAny<DbProject>()))
                .Throws(new Exception())
                .Verifiable();

            Assert.Throws<Exception>(() => command.Execute(newRequest));
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            repositoryMock.Verify();
            mapperMock.Verify();
        }

        [Test]
        public void ShouldReturnIdWhenCreatingNewProjectAndUserIsAdmin()
        {
            accessValidator
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);
            Assert.AreEqual(newProject.Id, command.Execute(newRequest));
            mapperMock.Verify();
            repositoryMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        }

        [Test]
        public void ShouldReturnIdWhenCreatingNewProjectAndUserHasRights()
        {
            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(false);

            Assert.AreEqual(newProject.Id, command.Execute(newRequest));
            mapperMock.Verify();
            repositoryMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        }

        [Test]
        public void ShouldReturnExceptionWhenUserIsNotAdminAndNotHasRights()
        {
            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(false);

            accessValidator
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            Assert.Throws<ForbiddenException>(() => command.Execute(newRequest));
            mapperMock.Verify(x => x.Map(It.IsAny<ProjectExpandedRequest>()), Times.Never);
            repositoryMock.Verify(x => x.CreateNewProject(It.IsAny<DbProject>()), Times.Never);
            validatorMock.Verify(x => x.Validate(It.IsAny<IValidationContext>()), Times.Never);
        }

        [Test]
        public void ShouldReturnIdWhenCreatingNewProject()
        {
            Assert.AreEqual(newProject.Id, command.Execute(newRequest));
            mapperMock.Verify();
            repositoryMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        }
    }
}
