using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.Responses
{
    internal class ProjectResponseMapperTests
    {
        private IProjectResponseMapper _projectIProjectResponseMapper;
        private Mock<IProjectInfoMapper> _projectInfoMapperMock;

        private DbProject _dbProject;
        private ProjectInfo _projectInfo;
        private IEnumerable<ProjectUserInfo> _users;
        private IEnumerable<ProjectFileInfo> _files;
        private DepartmentInfo _department;
        private List<string> _errors;
        private ProjectResponse _expectedResponse;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _projectInfoMapperMock = new Mock<IProjectInfoMapper>();

            _dbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Name = "Project for Lanit-Tercom",
                ShortName = "Project",
                Description = "New project for Lanit-Tercom",
                ShortDescription = "Short description",
                DepartmentId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Status = (int)ProjectStatusType.Active
            };

            _users = new List<ProjectUserInfo>
            {
                new ProjectUserInfo
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Spartak",
                    LastName = "Ryabtsev",
                    MiddleName = "Alexandrovich",
                    AddedOn = DateTime.UtcNow,
                    RemovedOn = DateTime.UtcNow,
                    IsActive = true,
                    Role = ProjectUserRoleType.Admin
                }
            };

            _files = new List<ProjectFileInfo>
            {
                new ProjectFileInfo
                {
                    FileId = Guid.NewGuid(),
                    ProjectId = _dbProject.Id
                }
            };

            _department = new DepartmentInfo
            {
                Id = _dbProject.DepartmentId.Value,
                Name = "DepartmentName"
            };

            _projectInfo = new ProjectInfo
            {
                Id = _dbProject.Id,
                Department = _department,
                Description = _dbProject.Description,
                ShortDescription = _dbProject.ShortDescription,
                Name = _dbProject.Name,
                AuthorId = _dbProject.AuthorId,
                CreatedAt = _dbProject.CreatedAt,
                ShortName = _dbProject.ShortName,
                Status =(ProjectStatusType)_dbProject.Status
            };

            _errors = new List<string> { "Error!!!" };

            _expectedResponse = new ProjectResponse
            {
                Project = _projectInfo,
                Users = _users,
                Files = _files,
                Errors = _errors
            };

            _projectInfoMapperMock
                .Setup(x => x.Map(_dbProject, _department.Name))
                .Returns(_projectInfo);

            _projectIProjectResponseMapper = new ProjectResponseMapper(_projectInfoMapperMock.Object);
        }

        [Test]
        public void ShoulThrowExceptionWhenDbProjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _projectIProjectResponseMapper.Map(null, _users, _files, _department, _errors));
        }

        [Test]
        public void ShoulThrowExceptionWhenDepartmentIdDontMatch()
        {
            _department.Id = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => _projectIProjectResponseMapper.Map(_dbProject, _users, _files, _department, _errors));
        }

        [Test]
        public void ShouldReturnProjectResponse()
        {
            var result = _projectIProjectResponseMapper.Map(_dbProject, _users, _files, _department, _errors);

            SerializerAssert.AreEqual(_expectedResponse, result);
        }
    }
}
