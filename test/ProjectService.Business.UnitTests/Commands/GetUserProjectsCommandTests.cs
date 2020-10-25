using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ProjectService.Business.UnitTests.Commands
{
    public class GetUserProjectsCommandTests
    {
        private GetUserProjectsCommand command;
        private Mock<IProjectRepository> repositoryMock;
        private Guid userId;
        private List<DbProject> projectsList;

        [OneTimeSetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IProjectRepository>();

            command = new GetUserProjectsCommand(
                repositoryMock.Object,
                new ProjectMapper());

            projectsList = new List<DbProject>();
            projectsList.Add(new DbProject());
            projectsList.Add(new DbProject());

            userId = Guid.NewGuid();
        }

        [Test]
        public void ShouldReturnListOfProjects()
        {
            var expected = new List<Project>()
            {
                new Project(),
                new Project()
            };

            repositoryMock
                .Setup(x => x.GetUserProjects(It.IsAny<Guid>()))
                .Returns(projectsList);

            var result = command.Execute(userId);

            SerializerAssert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsIt()
        {
            repositoryMock
                .Setup(x => x.GetUserProjects(It.IsAny<Guid>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(userId));
        }
    }
}
