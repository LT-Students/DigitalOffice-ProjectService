using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands
{
    class DisableWorkersInProjectCommandTests
    {
        private IDisableWorkersInProjectCommand command;

        private Mock<IProjectRepository> repositoryMock;
        private Mock<IAccessValidator> accessValidatorMock;

        private Guid projectId;
        private IEnumerable<Guid> userIds;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            projectId = Guid.NewGuid();
            userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        }

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IProjectRepository>();
            accessValidatorMock = new Mock<IAccessValidator>();

            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(true);

            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(true);

            repositoryMock
                .Setup(x => x.DisableWorkersInProject(projectId, userIds));

            command = new DisableWorkersInProjectCommand(repositoryMock.Object, accessValidatorMock.Object);
        }

        [Test]
        public void ShouldDisableWorkersSuccess()
        {
            Assert.DoesNotThrow(() => command.Execute(projectId, userIds));

            repositoryMock.Verify(repository =>
            repository.DisableWorkersInProject(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()), Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenUsersIsNull()
        {
            Assert.Throws<BadRequestException>(() => command.Execute(projectId, null));

            repositoryMock.Verify(repository =>
            repository.DisableWorkersInProject(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenUsersNotSpecified()
        {
            Assert.Throws<BadRequestException>(() => command.Execute(projectId, new List<Guid>()));

            repositoryMock.Verify(repository =>
            repository.DisableWorkersInProject(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsIt()
        {

            repositoryMock
                .Setup(x => x.DisableWorkersInProject(projectId, userIds))
                .Throws(new NullReferenceException());

            Assert.Throws<NullReferenceException>(() => command.Execute(projectId, userIds));
        }

        [Test]
        public void ShouldDisableWorkersSuccessWhenUserIsAdmin()
        {
            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            Assert.DoesNotThrow(() => command.Execute(projectId, userIds));
        }

        [Test]
        public void ShouldDisableWorkersWhenUserIsNotAdminAndHasRights()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(false);

            Assert.DoesNotThrow(() => command.Execute(projectId, userIds));
        }

        [Test]
        public void ShouldThrowExceptionWhenUserIsNotAdminAndHasNotRights()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(false);

            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            Assert.That(() => command.Execute(projectId, userIds), Throws.InstanceOf<Exception>());

            repositoryMock.Verify(repository =>
            repository.DisableWorkersInProject(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()), Times.Never);
        }
    }
}
