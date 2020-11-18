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

        private DbProject dbProject1;
        private DbProject dbProject2;

        private DbProjectUser userWithOneProject;
        private DbProjectUser userWithTwoProjects;
        private DbProjectUser userWithoutProject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            userWithOneProject = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                IsActive = true
            };

            userWithTwoProjects = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                IsActive = true
            };

            userWithoutProject = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                IsActive = true
            };
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase("InMemoryDatabase")
                .Options;

            provider = new ProjectServiceDbContext(dbOptions);
            repository = new ProjectRepository(provider);

            dbProject1 = new DbProject
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                Name = "Prroject1"
            };

            dbProject1.Users.Add(userWithOneProject);
            dbProject1.Users.Add(userWithTwoProjects);

            dbProject2 = new DbProject
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                Name = "Project2",
            };
            dbProject2.Users.Add(userWithTwoProjects);

            provider.Projects.Add(dbProject2);
            provider.Projects.Add(dbProject1);
            provider.Save();

        }

        [Test]
        public void ShouldReturnProjectListWithOneProject()
        {
            var result = repository.GetUserProjects(userWithOneProject.UserId, true);

            Assert.That(result, Is.EquivalentTo(new List<DbProject> { dbProject1 }));
        }

        [Test]
        public void ShouldReturnProjectListWithTwoProjects()
        {
            var result = repository.GetUserProjects(userWithTwoProjects.UserId, true);

            Assert.That(result, Is.EquivalentTo(new List<DbProject> { 
                dbProject1,
                //dbProject2 
            }));
        }

        [Test]
        public void ShouldReturnEmptyListWhenUserHaveNotProjects()
        {
            var result = repository.GetUserProjects(userWithoutProject.UserId, true);

            Assert.That(result, Is.EquivalentTo(new List<DbProject>()));
        }
    }
}