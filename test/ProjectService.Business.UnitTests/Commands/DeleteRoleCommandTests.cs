using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using Moq;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands
{
    class DeleteRoleCommandTests
    {
        private IDeleteRoleCommand command;

        private Mock<IRoleRepository> repositoryMock;
        private Mock<IAccessValidator> accessValidatorMock;

        private Guid roleId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            roleId = Guid.NewGuid();
        }
        
        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IRoleRepository>();
            accessValidatorMock = new Mock<IAccessValidator>();
            command = new DeleteRoleCommand(repositoryMock.Object, accessValidatorMock.Object);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsIt()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(true);

            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(true);

            repositoryMock
                .Setup(x => x.DeleteRole(roleId))
                .Throws(new NullReferenceException());

            Assert.Throws<NullReferenceException>(() => command.Execute(roleId));
            repositoryMock.Verify(repository => repository.DeleteRole(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public void ShouldDeletingRoleSuccess()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(true);

            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(true);

            repositoryMock
                .Setup(x => x.DeleteRole(roleId))
                .Returns(true);

            Assert.DoesNotThrow(() => command.Execute(roleId));
            repositoryMock.Verify(repository => repository.DeleteRole(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public void ShouldDeletingRoleSuccessWhenUserIsAdmin()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(true);

            repositoryMock
                .Setup(x => x.DeleteRole(roleId))
                .Returns(true);

            Assert.DoesNotThrow(() => command.Execute(roleId));
            repositoryMock.Verify(repository => repository.DeleteRole(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public void ShouldDeletingRoleSuccessWhenUserIsNotAdminAndHasRights()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(false);

            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(true);

            repositoryMock
                .Setup(x => x.DeleteRole(roleId))
                .Returns(true);

            Assert.DoesNotThrow(() => command.Execute(roleId));
            repositoryMock.Verify(repository => repository.DeleteRole(It.IsAny<Guid>()), Times.Once);
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

            Assert.That(() => command.Execute(roleId),
                Throws.InstanceOf<Exception>().And
                .Message.EqualTo("Not enough rights"));
            repositoryMock.Verify(repository => repository.DeleteRole(It.IsAny<Guid>()), Times.Never);
        }
    }
}
