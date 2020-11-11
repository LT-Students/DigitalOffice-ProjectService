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
        private List<DbProject> projectsEnum;

        private Guid userId;

        private DbProject project1;
        private DbProject project2;


        [OneTimeSetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IProjectRepository>();
            mapperMock = new Mock<IProjectResponseMapper>();
            command = new GetUserProjectsCommand(repositoryMock.Object, mapperMock.Object);

            project1 = new DbProject();
            project2 = new DbProject();

            projectsEnum = new List<DbProject>{
                project1,
                project2
            };
            Guid userId = Guid.NewGuid();


        }
        
        [Test]
        public void ShouldReturnListOfProjects()
        {
            var expected = new List<ProjectResponse> {new ProjectResponse (), new ProjectResponse ()};

            repositoryMock
                .Setup(x => x.GetUserProjects(It.IsAny<Guid>(), true))
                .Returns(projectsEnum)
                .Verifiable();

            mapperMock
                .Setup(x => x.Map(It.IsAny<DbProject>()))
                .Returns(new ProjectResponse ())
                .Verifiable();
   

            var result = command.Execute(userId, true);

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
