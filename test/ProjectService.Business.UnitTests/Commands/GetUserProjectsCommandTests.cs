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

        private DbProject projectOne;
        private DbProject projectTwo;
        private List<DbProject> projectsEnum;

        private ProjectResponse responseOne;
        private ProjectResponse responseTwo;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IProjectRepository>();
            mapperMock = new Mock<IProjectResponseMapper>();
            command = new GetUserProjectsCommand(repositoryMock.Object, mapperMock.Object);

            userId = Guid.NewGuid();

            projectOne = new DbProject();
            projectTwo = new DbProject();
            projectsEnum = new List<DbProject>();
            projectsEnum.Add(projectOne);
            projectsEnum.Add(projectTwo);
  
            responseOne = new ProjectResponse();
            responseTwo = new ProjectResponse();
        }
        
        [Test]
        public void ShouldReturnListOfProjects()
        {
            var expected = new List<ProjectResponse>();
            expected.Add(responseOne);
            expected.Add(responseTwo);

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
                .Throws(new Exception())
                .Verifiable();

            mapperMock.Verify(x => x.Map(It.IsAny<DbProject>()), Times.Never);

            Assert.Throws<Exception>(() => command.Execute(userId, true));
        }
    }
}
