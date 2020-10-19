using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.Kernel.UnitTestLibrary;

namespace ProjectService.Business.UnitTests.Commands
{
    class GetUserProjectsCommandTests
    {
        private GetUserProjectsCommand command;
        private Mock<IProjectRepository> repositoryMock;
        private Guid userId;
        private List<DbProject> projectsList;
        private DbProject project1;
        private DbProject project2;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IProjectRepository>();
            command = new GetUserProjectsCommand(repositoryMock.Object);
            projectsList = new List<DbProject>();
            project1 = new DbProject();
            project2 = new DbProject();
            projectsList.Add(project1);
            projectsList.Add(project2);
            userId = Guid.NewGuid();
        }
        [Test]
        public void ShouldReturnListOfProjects()
        {
            var expected = projectsList;
            repositoryMock.Setup(x => x.GetUserProjects(It.IsAny<Guid>())).Returns(projectsList);
            var result = command.Execute(userId);
            SerializerAssert.AreEqual(expected, result);    
        }
        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsIt()
        {
            repositoryMock.Setup(x => x.GetUserProjects(It.IsAny<Guid>())).Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(userId));
        }
    }
}
