using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.Commands.UnitTests
{
    internal class GetProjectByIdCommandTests
    {
        private IGetProjectByIdCommand command;
        private Mock<IProjectRepository> repositoryMock;
        private Mock<IProjectExpandedResponseMapper> mapperMock;

        private const string NAME = "Project";

        private const bool SHOW_NOT_ACTIVE_USERS = false;

        private IEnumerable<DbProjectUser> dbProjecUsers;
        private DbProject dbProject;

        private Guid projectId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            projectId = Guid.NewGuid();

            dbProject = new DbProject
            {
                Id = projectId,
                Name = NAME
            };

            dbProjecUsers = new List<DbProjectUser>
            {
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    Project = dbProject,
                    UserId = Guid.NewGuid()
                },

                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    Project = dbProject,
                    UserId = Guid.NewGuid()
                }
            };
        }

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IProjectRepository>();
            mapperMock = new Mock<IProjectExpandedResponseMapper>();
            command = new GetProjectByIdCommand(repositoryMock.Object, mapperMock.Object);
        }

        [Test]
        public void ShouldThrowExceptionWhenGetProjectFromRepository()
        {
            repositoryMock
                .Setup(x => x.GetProject(projectId))
                .Throws(new Exception())
                .Verifiable();

            Assert.ThrowsAsync<Exception>(() => command.Execute(projectId, SHOW_NOT_ACTIVE_USERS));
            repositoryMock.Verify();
        }

        [Test]
        public void ShouldThrowExceptionWhenGetProjectUsersFromRepository()
        {
            repositoryMock
                .Setup(x => x.GetProject(projectId))
                .Returns(dbProject)
                .Verifiable();

            repositoryMock
                .Setup(x => x.GetProjectUsers(projectId, SHOW_NOT_ACTIVE_USERS))
                .Throws(new Exception())
                .Verifiable();

            Assert.ThrowsAsync<Exception>(() => command.Execute(projectId, SHOW_NOT_ACTIVE_USERS));
            repositoryMock.Verify();
        }

        [Test]
        public void ShouldThrowExceptionWhenMapperThrowsIt()
        {
            repositoryMock
                .Setup(x => x.GetProject(projectId))
                .Returns(dbProject)
                .Verifiable();

            repositoryMock
                .Setup(x => x.GetProjectUsers(projectId, SHOW_NOT_ACTIVE_USERS))
                .Returns(dbProjecUsers)
                .Verifiable();

            mapperMock
                .Setup(x => x.Map(dbProject, dbProjecUsers))
                .Throws(new Exception())
                .Verifiable();

            Assert.ThrowsAsync<Exception>(() => command.Execute(projectId, SHOW_NOT_ACTIVE_USERS));
            repositoryMock.Verify();
            mapperMock.Verify();
        }

        [Test]
        public void ShouldReturnProjectInfo()
        {
            var expectedResult = new ProjectExpandedResponse
            {
                Project = new Project
                {
                    Id = dbProject.Id
                },
                Department = new Department
                {
                    Id = Guid.NewGuid()
                }
            };

            repositoryMock
                .Setup(x => x.GetProject(projectId))
                .Returns(dbProject)
                .Verifiable();

            repositoryMock
                .Setup(x => x.GetProjectUsers(projectId, SHOW_NOT_ACTIVE_USERS))
                .Returns(dbProjecUsers)
                .Verifiable();

            mapperMock
                .Setup(x => x.Map(dbProject, dbProjecUsers))
                .Returns(Task.FromResult(expectedResult))
                .Verifiable();

            var result = command.Execute(projectId, SHOW_NOT_ACTIVE_USERS).Result;

            Assert.AreEqual(expectedResult.Project.Id, result.Project.Id);
           // Assert.AreEqual(expectedResult.Project.Name, result.Project.Name);
            Assert.AreEqual(expectedResult.Department.Id, result.Department.Id);
            repositoryMock.Verify();
            mapperMock.Verify();
        }
    }
}