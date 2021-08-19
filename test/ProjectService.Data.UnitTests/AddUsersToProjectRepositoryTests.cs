using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
    internal class AddUsersToProjectRepositoryTests
    {
        private IDataProvider _provider;
        private IUserRepository _userRepository;
        private IProjectRepository _projectRepository;
        private Mock<IHttpContextAccessor> _accessorMock;

        private List<DbProjectUser> _newProjectUsers;
        private DbContextOptions<ProjectServiceDbContext> _dbOptionsProjectService;

        private Guid _projectId;
        private Guid _creatorId = Guid.NewGuid();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _projectId = Guid.NewGuid();

            _dbOptionsProjectService = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase("ProjectServiceTest")
                .Options;

            _newProjectUsers = new List<DbProjectUser>
            {
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = _projectId,
                    UserId = Guid.NewGuid(),
                    CreatedAtUtc = DateTime.Now,
                    CreatedBy = _creatorId,
                    IsActive = true
                },
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = _projectId,
                    UserId = Guid.NewGuid(),
                    CreatedAtUtc = DateTime.Now,
                    CreatedBy = _creatorId,
                    IsActive = true
                }

            };
        }

        [SetUp]
        public void SetUp()
        {
            _accessorMock = new();
            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", _creatorId);

            _accessorMock
                .Setup(x => x.HttpContext.Items)
                .Returns(_items);

            _provider = new ProjectServiceDbContext(_dbOptionsProjectService);

            _userRepository = new UserRepository(_provider);
            _projectRepository = new ProjectRepository(_provider, _accessorMock.Object);

            var newProject = new DbProject
            {
                Id = _projectId
            };

            _projectRepository.CreateNewProject(newProject);
        }

        [Test]
        public void ShouldArgumentNullExceptionWhenListDbProjectUserIsNull()
        {
            List<DbProjectUser> newProjectUsers = null;

            Assert.Throws<ArgumentNullException>(() => _userRepository.AddUsersToProject(newProjectUsers, _projectId));
        }

        // [Test]
        // public void ShouldBadRequestExceptionWhenProjectIdNotExist()
        // {
        //     var projectId = Guid.NewGuid();
        //
        //     Assert.Throws<BadRequestException>(() => _userRepository.AddUsersToProject(_newProjectUsers, projectId));
        // }

        [Test]
        public void ShouldAddNewUsersToProjectSuccessful()
        {
            Assert.DoesNotThrow(() => _userRepository.AddUsersToProject(_newProjectUsers, _projectId));
        }

        [TearDown]
        public void CleanMemoryDb()
        {
            if (_provider.IsInMemory())
            {
                _provider.EnsureDeleted();
            }
        }
    }
}
