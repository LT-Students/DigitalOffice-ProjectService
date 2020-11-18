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
        private Guid userIdWithoutProjects;

        private DbProject dbProject1;
        private DbProject dbProject2;

        private DbProjectUser dbUserWithOneProject;
        private DbProjectUser dbUserWithTwoProjects1;
        private DbProjectUser dbUserWithTwoProjects2;

        [SetUp]
        public void SetUp()
        {
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase("InMemoryDatabase")
                .Options;

            provider = new ProjectServiceDbContext(dbOptions);
            repository = new ProjectRepository(provider);

            userIdWithOneProject = Guid.NewGuid();
            userIdWithTwoProjects = Guid.NewGuid();
            userIdWithoutProjects = Guid.NewGuid();

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

            provider.Projects.Add(dbProject2);
            provider.Projects.Add(dbProject1);
            provider.Save();
        }

        [Test]
        public void ShouldReturnProjectListWithOneProject()
        {
            var result = repository.GetUserProjects(userIdWithOneProject, true);

            Assert.That(result, Is.EquivalentTo(new List<DbProject> { dbProject1 }));
        }

        [Test]
        public void ShouldReturnProjectListWithTwoProjects()
        {
            var result = repository.GetUserProjects(userIdWithTwoProjects, true);

            Assert.That(result, Is.EquivalentTo(new List<DbProject> { 
                dbProject1,
                dbProject2 
            }));
        }

        [Test]
        public void ShouldReturnEmptyListWhenUserHaveNotProjects()
        {
            var result = repository.GetUserProjects(userIdWithoutProjects, true);

            Assert.That(result, Is.EquivalentTo(new List<DbProject>()));
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