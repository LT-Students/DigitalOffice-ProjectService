using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.UnitTests
{
    internal class ProjectExpandedRequestMapperTests
    {
        private IProjectExpandedRequestMapper _projectRequestMapper;
        private Mock<IProjectUserRequestMapper> _projectUserRequestMapperMock;

        private ProjectExpandedRequest _projectRequest;
        private List<DbProjectUser> _expectedDbProjectUser;

        private Project _newProject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _projectUserRequestMapperMock = new Mock<IProjectUserRequestMapper>();
            _projectRequestMapper = new ProjectExpandedRequestMapper(_projectUserRequestMapperMock.Object);

            _newProject = new Project
            {
                DepartmentId = Guid.NewGuid(),
                ShortName = "DO",
                Description = "New project for Lanit-Tercom",
                CreatedAt = DateTime.Now,
                IsActive = true,
                Name = "12DigitalOffice24322525"
            };

            _projectRequest = new ProjectExpandedRequest
            {
                Project = _newProject,
                Users = new List<ProjectUserRequest>
                {
                    new ProjectUserRequest
                    {
                        User = new UserRequest
                        {
                            Id = Guid.NewGuid()
                        }
                    },
                    new ProjectUserRequest
                    {
                        User = new UserRequest
                        {
                            Id = Guid.NewGuid()
                        }
                    }
                }
            };

            _expectedDbProjectUser = new List<DbProjectUser>
            {
                new DbProjectUser
                {
                    UserId = _projectRequest.Users.ElementAt(0).User.Id
                },
                new DbProjectUser
                {
                    UserId = _projectRequest.Users.ElementAt(1).User.Id
                }
            };
        }

        [SetUp]
        public void SetUp()
        {
            _projectUserRequestMapperMock
                .SetupSequence(x => x.Map(It.IsAny<ProjectUserRequest>()))
                .Returns(new DbProjectUser
                {
                    UserId = _projectRequest.Users.ElementAt(0).User.Id
                })
                .Returns(new DbProjectUser
                {
                    UserId = _projectRequest.Users.ElementAt(1).User.Id
                });
        }

        [Test]
        public void ShouldReturnDbProjectWhenProjectRequestIsMappedAndProjectIdIsEmpty()
        {
            _projectRequest.Project.Id = null;

            var expectedDbProject = new DbProject
            {
                ShortName = _projectRequest.Project.ShortName,
                DepartmentId = _projectRequest.Project.DepartmentId,
                Name = _projectRequest.Project.Name,
                Description = _projectRequest.Project.Description,
                IsActive = _projectRequest.Project.IsActive,
                Users = _expectedDbProjectUser,
            };

            var dbProject = _projectRequestMapper.Map(_projectRequest);

            expectedDbProject.Id = dbProject.Id;
            expectedDbProject.Users.ElementAt(0).ProjectId = expectedDbProject.Id;
            expectedDbProject.Users.ElementAt(1).ProjectId = expectedDbProject.Id;

            SerializerAssert.AreEqual(expectedDbProject, dbProject);
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenProjectRequestIsNull()
        {
            ProjectExpandedRequest projectRequest = null;

            Assert.Throws<ArgumentNullException>(() => _projectRequestMapper.Map(projectRequest));
        }

        [Test]
        public void ShouldReturnDbProjectWhenProjectRequestIsMappedWithoutUsers()
        {
            var projectRequest = new ProjectExpandedRequest
            {
                Project = _newProject
            };

            var expectedDbProject = new DbProject
            {
                ShortName = projectRequest.Project.ShortName,
                DepartmentId = projectRequest.Project.DepartmentId,
                Name = projectRequest.Project.Name,
                Description = projectRequest.Project.Description,
                IsActive = projectRequest.Project.IsActive,
                Users = null
            };

            var dbProject = _projectRequestMapper.Map(projectRequest);

            expectedDbProject.Id = dbProject.Id;

            SerializerAssert.AreEqual(expectedDbProject, dbProject);
        }

        [Test]
        public void ShouldReturnDbProjectWhenProjectRequestIsMapped()
        {

            _projectRequest.Project.Id = Guid.NewGuid();

            var expectedDbProject = new DbProject
            {
                Id = (Guid)_projectRequest.Project.Id,
                ShortName = _projectRequest.Project.ShortName,
                DepartmentId = _projectRequest.Project.DepartmentId,
                Name = _projectRequest.Project.Name,
                Description = _projectRequest.Project.Description,
                IsActive = _projectRequest.Project.IsActive,
                Users = _expectedDbProjectUser,
            };

            expectedDbProject.Users.ElementAt(0).ProjectId = expectedDbProject.Id;
            expectedDbProject.Users.ElementAt(1).ProjectId = expectedDbProject.Id;

            var dbProject = _projectRequestMapper.Map(_projectRequest);

            SerializerAssert.AreEqual(expectedDbProject, dbProject);
        }
    }
}
