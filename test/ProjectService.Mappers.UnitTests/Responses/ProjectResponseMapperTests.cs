using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
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
        private IEnumerable<UserInfo> _users;
        private IEnumerable<FileIdentifier> _files;
        private DepartmentInfo _department;
        private ProjectResponse _expectedResponse;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _projectInfoMapperMock = new Mock<IProjectInfoMapper>();

            _dbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                CreatedBy = Guid.NewGuid(),
                Name = "Project for Lanit-Tercom",
                ShortName = "Project",
                Description = "New project for Lanit-Tercom",
                ShortDescription = "Short description",
                CreatedAtUtc = DateTime.UtcNow,
                Status = (int)ProjectStatusType.Active
            };

            _users = new List<UserInfo>
            {
                new UserInfo
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Spartak",
                    LastName = "Ryabtsev",
                    MiddleName = "Alexandrovich",
                    CreatedAtUtc = DateTime.UtcNow,
                    ModifiedAtUtc = DateTime.UtcNow,
                    IsActive = true,
                    Role = ProjectUserRoleType.Manager
                }
            };

            _files = new List<FileIdentifier>
            {
                new FileIdentifier
                {
                    FileId = Guid.NewGuid(),
                    ProjectId = _dbProject.Id
                }
            };

            _department = new DepartmentInfo
            {
                Id = Guid.NewGuid(),
                Name = "DepartmentName"
            };

            _projectInfo = new ProjectInfo
            {
                Id = _dbProject.Id,
                Department = _department,
                Description = _dbProject.Description,
                ShortDescription = _dbProject.ShortDescription,
                Name = _dbProject.Name,
                CreatedBy = _dbProject.CreatedBy,
                CreatedAtUtc = _dbProject.CreatedAtUtc,
                ShortName = _dbProject.ShortName,
                Status =(ProjectStatusType)_dbProject.Status
            };

            _expectedResponse = new ProjectResponse
            {
                Project = _projectInfo,
                Users = _users
            };

            _projectInfoMapperMock
                .Setup(x => x.Map(_dbProject, _department))
                .Returns(_projectInfo);

            _projectIProjectResponseMapper = new ProjectResponseMapper(_projectInfoMapperMock.Object);
        }

        /*[Test]
        public void ShoulThrowExceptionWhenDbProjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _projectIProjectResponseMapper.Map(null, _users, _files, _department));
        }

        [Test]
        public void ShoulThrowExceptionWhenDepartmentIdDontMatch()
        {
            _department.Id = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => _projectIProjectResponseMapper.Map(_dbProject, _users, _files, _department));
        }

        [Test]
        public void ShouldReturnProjectResponse()
        {
            var result = _projectIProjectResponseMapper.Map(_dbProject, _users, _files, _department);

            SerializerAssert.AreEqual(_expectedResponse, result);
        }*/
    }
}
