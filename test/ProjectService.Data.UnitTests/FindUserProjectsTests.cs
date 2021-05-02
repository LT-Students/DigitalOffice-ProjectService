using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
    class FIndUserProjectsTests
    {
        private IDataProvider _provider;
        private IUserRepository _userRepository;

        private List<DbProject> _projects;
        private List<DbProjectUser> _newProjectUsers;

        private Guid _userId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _userId = Guid.NewGuid();

            _projects = new List<DbProject>
            {
                new DbProject
                {
                    Id = Guid.NewGuid(),
                    Name = "Digital Office",
                    ShortName = "DO",
                    CreatedAt = DateTime.UtcNow,
                    AuthorId = Guid.NewGuid(),
                    Status = 1
                },
                new DbProject
                {
                    Id = Guid.NewGuid(),
                    Name = "Digital Office",
                    ShortName = "DO",
                    CreatedAt = DateTime.UtcNow,
                    AuthorId = Guid.NewGuid(),
                    Status = 1
                }
            };

            _newProjectUsers = new List<DbProjectUser>
            {
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = _projects[0].Id,
                    UserId = _userId,
                    AddedOn = DateTime.Now,
                    IsActive = true
                },
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = _projects[1].Id,
                    UserId = _userId,
                    AddedOn = DateTime.UtcNow,
                    IsActive = true
                }
            };
        }

        [SetUp]
        public void SetUp()
        {
            var dbOptionsProjectService = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase("ProjectServiceTest")
                .Options;

            _provider = new ProjectServiceDbContext(dbOptionsProjectService);

            _userRepository = new UserRepository(_provider);

            _provider.Projects.AddRange(_projects);
            _provider.ProjectsUsers.AddRange(_newProjectUsers);
            _provider.Save();
        }

        [Test]
        public void ShouldReturnsDbProjectUsersWhenListDbProjectUserIsNull()
        {
            var result = _userRepository.Find(_userId).Select(x => { x.Project = null; return x; });
            var expected = _newProjectUsers.Select(x => { x.Project = null; return x; });

            SerializerAssert.AreEqual(expected, result);
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
