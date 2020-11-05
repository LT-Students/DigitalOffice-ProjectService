using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
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
        private IEnumerable<ProjectResponse> projectsEnum;
    
        [OneTimeSetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IProjectRepository>();
            mapperMock = new Mock<IProjectResponseMapper>();
            command = new GetUserProjectsCommand(repositoryMock.Object, mapperMock.Object);

            projectsEnum {
                new ProjectResponse(), 
                new ProjectResponse()
            };
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
