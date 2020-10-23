using FluentValidation;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;
using Moq;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Commands
{
    public class EditProjectByIdCommandTests
    {
        private Guid projectId;
        private DbProject dbProject;
        private EditProjectRequest editRequest;
        private IEditProjectByIdCommand command;
        private Mock<IProjectRepository> repositoryMock;
        private Mock<IValidator<EditProjectRequest>> validatorMock;
        private Mock<IMapper<EditProjectRequest, DbProject>> mapperMock;

        [SetUp]
        public void SetUp()
        {
            projectId = Guid.NewGuid();

            validatorMock = new Mock<IValidator<EditProjectRequest>>();
            repositoryMock = new Mock<IProjectRepository>();
            mapperMock = new Mock<IMapper<EditProjectRequest, DbProject>>();

            command = new EditProjectByIdCommand(mapperMock.Object, validatorMock.Object, repositoryMock.Object);

            editRequest = new EditProjectRequest
            {
                Id = projectId,
                Name = "DigitalOffice",
                ShortName = "DO",
                DepartmentId = Guid.NewGuid(),
                Description = "A new description",
                IsActive = false
            };

            dbProject = new DbProject
            {
                Id = projectId,
                Name = editRequest.Name,
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
                () => command.Execute(projectId, editRequest), "Validator did not throw a ValidationException");

            mapperMock.Verify(m => m.Map(editRequest), Times.Never);
            repositoryMock.Verify(r => r.EditProjectById(dbProject), Times.Never);
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenDbProjectWasNotFound()
        {
            validatorMock
                 .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                 .Returns(true);

            mapperMock
                .Setup(x => x.Map(It.IsAny<EditProjectRequest>()))
                .Returns(dbProject);

            repositoryMock
                .Setup(x => x.EditProjectById(It.IsAny<DbProject>()))
                .Throws(new NullReferenceException());

            Assert.Throws<NullReferenceException>(
                () => command.Execute(projectId, editRequest), "Project with this ID has been found");
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
                .Setup(x => x.Map(It.IsAny<EditProjectRequest>()))
                .Returns(dbProject);

            repositoryMock
                .Setup(x => x.EditProjectById(It.IsAny<DbProject>()))
                .Returns(projectId);

            Assert.Throws<ArgumentNullException>(
                () => command.Execute(projectId, editRequest), "Argument was not null");
            validatorMock.Verify(v => v.Validate(It.IsAny<IValidationContext>()), Times.Never);
            mapperMock.Verify(m => m.Map(It.IsAny<EditProjectRequest>()), Times.Never);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenProjectGuidIsEmpty()
        {
            validatorMock
                 .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                 .Returns(false);

            mapperMock
                .Setup(x => x.Map(It.IsAny<EditProjectRequest>()))
                .Returns(dbProject);

            repositoryMock
                .Setup(x => x.EditProjectById(It.IsAny<DbProject>()))
                .Returns(projectId);

            Assert.Throws<ValidationException>(
                () => command.Execute(projectId, editRequest), "Argument was not null");
            validatorMock.Verify(v => v.Validate(It.IsAny<IValidationContext>()), Times.Once);
            mapperMock.Verify(m => m.Map(It.IsAny<EditProjectRequest>()), Times.Never);
            repositoryMock.Verify(r => r.EditProjectById(dbProject), Times.Never);
        }

        [Test]
        public void ShouldReturnProjectGuidWhenProjectIsEdited()
        {
            validatorMock
                .Setup(v => v.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);
            mapperMock
                .Setup(m => m.Map(It.IsAny<EditProjectRequest>()))
                .Returns(dbProject);
            repositoryMock
                .Setup(r => r.EditProjectById(dbProject))
                .Returns(projectId);

            Assert.AreEqual(projectId, command.Execute(projectId, editRequest));
            validatorMock.Verify(v => v.Validate(It.IsAny<IValidationContext>()), Times.Once);
            repositoryMock.Verify(r => r.EditProjectById(dbProject), Times.Once);
            mapperMock.Verify(m => m.Map(editRequest), Times.Once);
        }
    }
}