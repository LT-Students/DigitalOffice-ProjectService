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

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
    class UserRepositoryTests
    {
        private IDataProvider _provider;
        private UserRepository _userRepository;
        private IProjectRepository _projectRepository;
        private DbContextOptions<ProjectServiceDbContext> _dbOptionsProjectService;

        private List<DbProject> _projects;
        private List<DbProjectUser> _projectsUser;

        private Guid _userId = Guid.NewGuid();
        private Guid _firstProject = Guid.NewGuid();
        private Guid _secondProject = Guid.NewGuid();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _projectsUser = new List<DbProjectUser>
            {
                new DbProjectUser
                {
                    UserId = _userId,
                    ProjectId = _firstProject
                },
                new DbProjectUser
                {
                    UserId = _userId,
                    ProjectId = _secondProject
                }
            };

            _projects = new List<DbProject>
            {
                new DbProject
                {
                    Id = _firstProject,
                    Users = new List<DbProjectUser> { _projectsUser[0] }
                },
                new DbProject
                {
                    Id = _secondProject,
                    Users = new List<DbProjectUser> { _projectsUser[1] }
                },
            };
        }

        [SetUp]
        public void SetUp()
        {
            _dbOptionsProjectService = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            _provider = new ProjectServiceDbContext(_dbOptionsProjectService);
            _userRepository = new UserRepository(_provider);
            _projectRepository = new ProjectRepository(_provider);
        }

        [Test]
        public void ShouldReturnDbProjectUser()
        {
            _provider.ProjectsUsers.AddRange(_projectsUser);
            _provider.Save();

            SerializerAssert.AreEqual(_projectsUser, _userRepository.Find(_userId));
        }

        [Test]
        public void ThrowExceptionWhenFilterIsNull()
        {
            FindDbProjectsUserFilter filter = null;

            Assert.Throws<ArgumentNullException>(() => _userRepository.Find(filter));
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
