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

        private Guid projectId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            projectId = Guid.NewGuid();

            var dbOptionsProjectService = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase("ProjectServiceTest")
                .Options;

            _provider = new ProjectServiceDbContext(dbOptionsProjectService);

            _userRepository = new UserRepository(_provider);
            _projectRepository = new ProjectRepository(_provider);

            _newProjectUsers = new List<DbProjectUser>
            {
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = Guid.NewGuid(),
                    RoleId = Guid.NewGuid(),
                    AddedOn = DateTime.Now,
                    IsActive = true
                },
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = Guid.NewGuid(),
                    RoleId = Guid.NewGuid(),
                    AddedOn = DateTime.Now,
                    IsActive = true
                }

            };
        }

        [SetUp]
        public void SetUp()
        {
            _userRepository = new UserRepository(_provider);
            _projectRepository = new ProjectRepository(_provider);

            var newProject = new DbProject
            {
                Id = projectId
            };

            _projectRepository.CreateNewProject(newProject);
        }

        [Test]
        public void ShouldArgumentNullExceptionWhenListDbProjectUserIsNull()
        {
            List<DbProjectUser> newProjectUsers = null;

            Assert.Throws<ArgumentNullException>(() => _userRepository.AddUsersToProject(newProjectUsers, projectId));
        }

        [Test]
        public void ShouldAddNewUsersToProjectSuccessful()
        {
            Assert.DoesNotThrow(() => _userRepository.AddUsersToProject(_newProjectUsers, projectId));
        }
    }
}
