using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using Moq;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Commands
{
    public class GetProjectByIdCommandTests
    {
        private IGetProjectCommand command;
        private Mock<IProjectRepository> repositoryMock;
        private Mock<IProjectExpandedResponseMapper> mapperMock;

        private DbProjectUser dbWorkersIds;
        private DbProject project;

        private Guid projectId;
        private Guid workerId;

        private const string NAME = "Project";

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IProjectRepository>();
            mapperMock = new Mock<IProjectExpandedResponseMapper>();
            command = new GetProjectCommand(repositoryMock.Object, mapperMock.Object);

            workerId = Guid.NewGuid();
            projectId = Guid.NewGuid();

            dbWorkersIds = new DbProjectUser
            {
                ProjectId = projectId,
                Project = project
            };

            project = new DbProject
            {
                Name = NAME
            };
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsIt()
        {
            repositoryMock.Setup(x => x.GetProject(It.IsAny<Guid>())).Throws(new Exception());

            //Assert.Throws<Exception>(() => command.Execute(projectId));
        }

        [Test]
        public void ShouldThrowExceptionWhenMapperThrowsIt()
        {
            //mapperMock.Setup(x => x.Map(It.IsAny<DbProject>())).Throws(new Exception());

            //Assert.Throws<Exception>(() => command.Execute(projectId));
        }

        [Test]
        public void ShouldReturnProjectInfo()
        {
            //var expected = new Project
            //{
            //    Name = project.Name,
            //};

            //repositoryMock
            //    .Setup(x => x.GetProject(It.IsAny<Guid>()))
            //    .Returns(project);
            //mapperMock
            //    .Setup(x => x.Map(It.IsAny<DbProject>()))
            //    .Returns(new ProjectExpandedResponse());

            //var result = command.Execute(projectId);

            //SerializerAssert.AreEqual(expected, result);
        }
    }
}