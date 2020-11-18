using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.UnitTestKernel;
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
        private Mock<IProjectResponseMapper> mapperMock;

        private Guid userId;

        private DbProject project1;
        private DbProject project2;
        private List<DbProject> projectsEnum;

        private ProjectResponse response1;
        private ProjectResponse response2;

        [OneTimeSetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IProjectRepository>();
            mapperMock = new Mock<IProjectResponseMapper>();
            command = new GetUserProjectsCommand(repositoryMock.Object, mapperMock.Object);

            userId = Guid.NewGuid();

            project1 = new DbProject();
            project2 = new DbProject();
            projectsEnum = new List<DbProject>();
            projectsEnum.Add(project1);
            projectsEnum.Add(project2);
  
            response1 = new ProjectResponse();
            response2 = new ProjectResponse();
        }
        
        [Test]
        public void ShouldReturnListOfProjects()
        {
            var expected = new List<ProjectResponse>();
            expected.Add(response1);
            expected.Add(response2);

            repositoryMock
                .Setup(x => x.GetUserProjects(It.IsAny<Guid>(), false))
                .Returns(projectsEnum)
                .Verifiable();

            mapperMock
                .Setup(x => x.Map(It.IsAny<DbProject>()))
                .Returns(new ProjectResponse())
                .Verifiable();
   

            var result = command.Execute(userId, false);

            SerializerAssert.AreEqual(expected, result);
            mapperMock.Verify();
            repositoryMock.Verify();
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsIt()
        {
            repositoryMock
                .Setup(x => x.GetUserProjects(It.IsAny<Guid>(), true))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(userId, true));
        }
    }
}
