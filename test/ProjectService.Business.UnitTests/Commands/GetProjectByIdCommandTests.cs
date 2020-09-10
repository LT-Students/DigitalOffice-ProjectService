using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Commands
{
    public class GetProjectByIdCommandTests
    {
        private IGetProjectInfoByIdCommand command;
        private Mock<IProjectRepository> repositoryMock;
        private Mock<IMapper<DbProject, Project>> mapperMock;

        private DbProjectWorkerUser dbWorkersIds;
        private DbProject project;

        private Guid projectId;
        private Guid workerId;

        private const string NAME = "Project";

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IProjectRepository>();
            mapperMock = new Mock<IMapper<DbProject, Project>>();
            command = new GetProjectByIdCommand(repositoryMock.Object, mapperMock.Object);

            workerId = Guid.NewGuid();
            projectId = Guid.NewGuid();
            dbWorkersIds = new DbProjectWorkerUser
            {
                ProjectId = projectId,
                Project = project,
                WorkerUserId = workerId
            };
            project = new DbProject
            {
                Name = NAME,
                WorkersUsersIds = new List<DbProjectWorkerUser> { dbWorkersIds }
            };
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsIt()
        {
            repositoryMock.Setup(x => x.GetProjectInfoById(It.IsAny<Guid>())).Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(projectId));
        }

        [Test]
        public void ShouldThrowExceptionWhenMapperThrowsIt()
        {
            mapperMock.Setup(x => x.Map(It.IsAny<DbProject>())).Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(projectId));
        }

        [Test]
        public void ShouldReturnProjectInfo()
        {
            var expected = new Project
            {
                Name = project.Name,
                WorkersIds = project.WorkersUsersIds?.Select(x => x.WorkerUserId).ToList()
            };

            repositoryMock
                .Setup(x => x.GetProjectInfoById(It.IsAny<Guid>()))
                .Returns(project);
            mapperMock
                .Setup(x => x.Map(It.IsAny<DbProject>()))
                .Returns(expected);

            var result = command.Execute(projectId);

            SerializerAssert.AreEqual(expected, result);
        }
    }
}