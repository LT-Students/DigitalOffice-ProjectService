using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
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
        private DbProjectFile _dbFile;
        private DbProjectUser _activeDbUser;
        private DbProjectUser _notActiveDbUser;
        private Guid _creatorId = Guid.NewGuid();
        private Mock<IHttpContextAccessor> _accessorMock;

        [SetUp]
        public void SetUp()
        {
            _accessorMock = new();
            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", _creatorId);

            _accessorMock
                .Setup(x => x.HttpContext.Items)
                .Returns(_items);

            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                                    .UseInMemoryDatabase("InMemoryDatabase")
                                    .Options;

            _provider = new ProjectServiceDbContext(dbOptions);
            _repository = new ProjectRepository(_provider, _accessorMock.Object);

            _dbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "Project"
            };

            _dbFile = new DbProjectFile
            {
                Id = Guid.NewGuid(),
                FileId = Guid.NewGuid(),
                ProjectId = _dbProject.Id,
            };

            _activeDbUser = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ProjectId = _dbProject.Id,
                IsActive = true
            };

            _notActiveDbUser = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ProjectId = _dbProject.Id,
                IsActive = false
            };

            _provider.Projects.Add(_dbProject);
            _provider.ProjectsUsers.AddRange(_activeDbUser, _notActiveDbUser);
            _provider.ProjectsFiles.Add(_dbFile);
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

            Assert.Throws<NotFoundException>(() => _repository.Get(notFoundFilter));
        }

        [Test]
        public void ShouldReturnProjectWithAllUsersAndFiles()
        {
            var fullFilter = new GetProjectFilter
            {
                ProjectId = _dbProject.Id,
                IncludeFiles = true,
                IncludeUsers = true,
                ShowNotActiveUsers = true
            };

            var result = _repository.Get(fullFilter);

            var expected = new DbProject
            {
                Id = _dbProject.Id,
                Name = _dbProject.Name,
                Files = new List<DbProjectFile> { _dbFile },
                Users = new List<DbProjectUser> { _activeDbUser, _notActiveDbUser }
            };

            Assert.AreEqual(expected.Id, result.Id);

            Assert.AreEqual(expected.Files.Count, result.Files.Count);
            Assert.AreEqual(expected.Files.First().Id, result.Files.First().Id);

            Assert.AreEqual(expected.Users.Count, result.Users.Count);
            Assert.NotNull(result.Users.First(x => x.Id == _activeDbUser.Id));
            Assert.NotNull(result.Users.First(x => x.Id == _notActiveDbUser.Id));
            Assert.NotNull(expected.Users.First(x => x.Id == _activeDbUser.Id));
            Assert.NotNull(expected.Users.First(x => x.Id == _notActiveDbUser.Id));
        }

        [Test]
        public void ShouldReturnProjectWithFiles()
        {
            var fullFilter = new GetProjectFilter
            {
                ProjectId = _dbProject.Id,
                IncludeFiles = false,
                IncludeUsers = false,
                ShowNotActiveUsers = true
            };

            var result = _repository.Get(fullFilter);

            var expected = new DbProject
            {
                Id = _dbProject.Id,
                Name = _dbProject.Name,
                Files = new List<DbProjectFile> { _dbFile }
            };

            Assert.AreEqual(expected.Id, result.Id);

            Assert.AreEqual(expected.Files.Count, result.Files.Count);
            Assert.AreEqual(expected.Files.First().Id, result.Files.First().Id);
        }
    }
}