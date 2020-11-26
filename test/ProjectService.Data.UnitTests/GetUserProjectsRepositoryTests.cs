using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
    public class GetUserProjectsRepositoryTests
    {
        private IDataProvider provider;
        private IProjectRepository repository;

        private Guid userIdWithOneProject;
        private Guid userIdWithTwoProjects;
        private Guid userIdWithoutActiveProjects;

        private DbProject dbProject1;
        private DbProject dbProject2;
        private DbProject dbNotActiveProject;

        private DbProjectUser dbUserWithOneProject;
        private DbProjectUser dbUserWithTwoProjects1;
        private DbProjectUser dbUserWithTwoProjects2;
        private DbProjectUser dbUserWithoutActiveProject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase("InMemoryDatabase")
                .Options;

            provider = new ProjectServiceDbContext(dbOptions);
            repository = new ProjectRepository(provider);

            userIdWithOneProject = Guid.NewGuid();
            userIdWithTwoProjects = Guid.NewGuid();
            userIdWithoutActiveProjects = Guid.NewGuid();

            dbUserWithOneProject = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                UserId = userIdWithOneProject,
                IsActive = true
            };

            dbUserWithTwoProjects1 = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                UserId = userIdWithTwoProjects,
                IsActive = true
            };

            dbUserWithTwoProjects2 = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                UserId = userIdWithTwoProjects,
                IsActive = true
            };

            dbUserWithoutActiveProject = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                UserId = userIdWithoutActiveProjects,
                IsActive = true
            };

            dbProject1 = new DbProject
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                Name = "Prroject1"
            };
            dbProject1.Users.Add(dbUserWithTwoProjects1);
            dbProject1.Users.Add(dbUserWithOneProject);

            dbProject2 = new DbProject
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                Name = "Project2",
            };
            dbProject2.Users.Add(dbUserWithTwoProjects2);

            dbNotActiveProject = new DbProject
            {
                Id = Guid.NewGuid(),
                IsActive = false,
                Name = "Project3",
            };
            dbNotActiveProject.Users.Add(dbUserWithoutActiveProject);
        }

        [SetUp]
        public void SetUp() {
            provider.Projects.Add(dbProject2);
            provider.Projects.Add(dbProject1);
            provider.Projects.Add(dbNotActiveProject);
            provider.Save();
        }

        [Test]
        public void ShouldReturnProjectListWithOneProject()
        {
            var result = repository.GetUserProjects(userIdWithOneProject, false);

            Assert.That(result, Is.EquivalentTo(new List<DbProject> { dbProject1 }));
        }

        [Test]
        public void ShouldReturnProjectListWithTwoProjects()
        {
            var result = repository.GetUserProjects(userIdWithTwoProjects, false);

            Assert.That(result, Is.EquivalentTo(new List<DbProject> { 
                dbProject1,
                dbProject2 
            }));
        }

        [Test]
        public void ShouldReturnEmptyListWhenUserHaveNotActiveProjects()
        {
            var result = repository.GetUserProjects(userIdWithoutActiveProjects, false);

            Assert.That(result, Is.EquivalentTo(new List<DbProject>()));
        }

        [Test]
        public void ShouldReturnOneNotActiveProjectWhenShowNotActiveIsTrue()
        {
            var result = repository.GetUserProjects(userIdWithoutActiveProjects, true);

            Assert.That(result, Is.EquivalentTo(new List<DbProject> { dbNotActiveProject }));
        }

        [TearDown]
        public void Clean()
        {
            if (provider.IsInMemory())
            {
                provider.EnsureDeleted();
            }
        }
    }
}