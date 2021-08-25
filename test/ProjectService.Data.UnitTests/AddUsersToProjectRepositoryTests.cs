using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore;
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

        private List<DbProjectUser> _newProjectUsers;
        private DbContextOptions<ProjectServiceDbContext> _dbOptionsProjectService;

        private Guid _projectId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _projectId = Guid.NewGuid();

            _dbOptionsProjectService = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase("ProjectServiceTest")
                .Options;

            _userRepository = new UserRepository(_provider);
            _projectRepository = new ProjectRepository(_provider);

            _newProjectUsers = new List<DbProjectUser>
            {
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = _projectId,
                    UserId = Guid.NewGuid(),
                    AddedOn = DateTime.Now,
                    IsActive = true
                },
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = _projectId,
                    UserId = Guid.NewGuid(),
                    AddedOn = DateTime.Now,
                    IsActive = true
                }

            };
        }

        [SetUp]
        public void SetUp()
        {
            _provider = new ProjectServiceDbContext(_dbOptionsProjectService);

            _userRepository = new UserRepository(_provider);
            _projectRepository = new ProjectRepository(_provider);

            var newProject = new DbProject
            {
                Id = _projectId
            };

            _projectRepository.Create(newProject);
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
