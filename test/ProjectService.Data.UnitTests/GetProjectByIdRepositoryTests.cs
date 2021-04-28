using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Repositories
{
    internal class GetProjectByIdRepositoryTests
    {
        private IDataProvider _provider;
        private IProjectRepository _repository;

        private DbProject _dbProject;

        [SetUp]
        public void SetUp()
        {
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                                    .UseInMemoryDatabase("InMemoryDatabase")
                                    .Options;

            _provider = new ProjectServiceDbContext(dbOptions);
            _repository = new ProjectRepository(_provider);

            _dbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "Project"
            };

            var dbFile = new DbProjectFile
            {
                Id = Guid.NewGuid(),
                FileId = Guid.NewGuid(),
                ProjectId = _dbProject.Id,
                Project = _dbProject
            };

            _dbProject.Files = new List<DbProjectFile> { dbFile };

            var activeDbUser = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ProjectId = _dbProject.Id,
                Project = _dbProject,
                IsActive = true
            };

            var notActiveDbUser = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ProjectId = _dbProject.Id,
                Project = _dbProject,
                IsActive = false
            };

            _dbProject.Users = new List<DbProjectUser> { activeDbUser, notActiveDbUser };

            _provider.Projects.Add(_dbProject);
            _provider.Save();
        }

        [TearDown]
        public void Clean()
        {
            if (_provider.IsInMemory())
            {
                _provider.EnsureDeleted();
            }
        }

        [Test]
        public void ShouldThrowExceptionWhenProjectDoesNotExist()
        {
            var notFoundFilter = new GetProjectFilter
            {
                ProjectId = Guid.NewGuid()
            };

            Assert.Throws<NotFoundException>(() => _repository.GetProject(notFoundFilter));
        }

        [Test]
        public void ShouldReturnFullProjectInfo()
        {
            var fullFilter = new GetProjectFilter
            {
                ProjectId = _dbProject.Id,
                IncludeFiles = true,
                IncludeUsers = true,
                ShowNotActiveUsers = true
            };

            var result = _repository.GetProject(fullFilter);

            var expected = new DbProject
            {
                Id = _dbProject.Id,
                Name = _dbProject.Name,
                Files = _dbProject.Files,
                Users = _dbProject.Users
            };

            SerializerAssert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldNotReturnInfo()
        {
            var fullFilter = new GetProjectFilter
            {
                ProjectId = _dbProject.Id,
                IncludeFiles = false,
                IncludeUsers = false,
                ShowNotActiveUsers = true
            };

            var result = _repository.GetProject(fullFilter);

            var expected = new DbProject
            {
                Id = _dbProject.Id,
                Name = _dbProject.Name
            };

            SerializerAssert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldReturnActiveUsersProjectInfo()
        {
            var fullFilter = new GetProjectFilter
            {
                ProjectId = _dbProject.Id,
                IncludeUsers = true,
                ShowNotActiveUsers = false
            };

            var result = _repository.GetProject(fullFilter);

            var expected = new DbProject
            {
                Id = _dbProject.Id,
                Name = _dbProject.Name,
                Users = _dbProject.Users.Where(x => x.IsActive == true).ToList()
            };

            SerializerAssert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldReturnMinimumInfo()
        {
            var fullFilter = new GetProjectFilter
            {
                ProjectId = _dbProject.Id
            };

            var result = _repository.GetProject(fullFilter);

            var expected = new DbProject
            {
                Id = _dbProject.Id,
                Name = _dbProject.Name
            };

            SerializerAssert.AreEqual(expected, result);
        }
    }
}