using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ProjectService.Data.UnitTests
{
    public class GetUserProjectsRepositoryTests
    {
        private IDataProvider provider;
        private IProjectRepository repository;

        private DbProject dbProjectOne;

        private DbProject dbProjectTwo;
        private List<DbProjectWorkerUser> listWorkersUsersIdsTwo;

        private DbProjectWorkerUser user1;
        private DbProjectWorkerUser user2;
        private DbProjectWorkerUser userWithoutProject;

        [SetUp]
        public void SetUp()
        {
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase("InMemoryDatabase").Options;

            provider = new ProjectServiceDbContext(dbOptions);
            repository = new ProjectRepository(provider);

            user1 = new DbProjectWorkerUser
            {
                //ProjectId,
                WorkerUserId = Guid.NewGuid(),
                IsActive = true
            };

            user2 = new DbProjectWorkerUser
            {
                //ProjectId,
                WorkerUserId = Guid.NewGuid(),
                IsActive = true
            };

            userWithoutProject = new DbProjectWorkerUser
            {
                WorkerUserId = Guid.NewGuid(),
                IsActive = true
            };

            listWorkersUsersIdsTwo = new List<DbProjectWorkerUser> { user1, user2 };

            dbProjectOne = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "Project1",
                WorkersUsersIds = new List<DbProjectWorkerUser> { user1 }
            };

            dbProjectTwo = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "Project2",
                WorkersUsersIds = listWorkersUsersIdsTwo
            };

            provider.Projects.Add(dbProjectOne);
            provider.Projects.Add(dbProjectTwo);
            provider.SaveModelsChanges();
        }

        [TearDown]
        public void Clean()
        {
            if (provider.IsInMemory())
            {
                provider.EnsureDeleted();
            }
        }

        [Test]
        public void ShouldReturnProjectListWithOneProject()
        {
            var result = repository.GetUserProjects(user2.WorkerUserId);

            Assert.That(result, Is.EquivalentTo(new List<DbProject> { dbProjectTwo }));
        }

        [Test]
        public void ShouldReturnEmptyListWhenUserHaveNotProjects()
        {
            var result = repository.GetUserProjects(userWithoutProject.WorkerUserId);

            Assert.That(result, Is.EquivalentTo(new List<DbProject>()));
        }

        [Test]
        public void ShouldThrowBadRequestException()
        {
            Assert.Throws<BadRequestException>(() => repository.GetUserProjects(Guid.Empty));
        }
    }
}
