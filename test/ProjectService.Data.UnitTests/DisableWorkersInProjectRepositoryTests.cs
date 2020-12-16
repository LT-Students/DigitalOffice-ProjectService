using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Repositories
{
    class DisableWorkersInProjectRepositoryTests
    {
        #region variables declaration
        private IDataProvider provider;
        private IProjectRepository repository;

        private DbProject newProject;
        private List<DbProjectUser> dbProjectUsers;
        private Guid projectIdRequest;
        private IEnumerable<Guid> userIdsRequest;
        #endregion

        #region setup
        private void CreateMemoryContext()
        {
            var dbOptionsProjectService = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase("ProjectServiceTest")
                .Options;

            provider = new ProjectServiceDbContext(dbOptionsProjectService);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var projectId = Guid.NewGuid();

            newProject = new DbProject
            {
                Id = projectId,
                Name = "DigitalOffice",
                ShortName = "DO",
                DepartmentId = Guid.NewGuid(),
                Description = "New project for Lanit-Tercom",
                IsActive = true,
                Users = new List<DbProjectUser>()
            };

            dbProjectUsers = new List<DbProjectUser>();

            for (int i = 0; i < 3; i++)
            {
                var dbProjectUser = new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = Guid.NewGuid(),
                    AddedOn = DateTime.Today,
                    RemovedOn = DateTime.Today,
                    IsActive = true
                };

                dbProjectUsers.Add(dbProjectUser);
            }

            newProject.Users = dbProjectUsers;

            projectIdRequest = newProject.Id;
            userIdsRequest = newProject.Users.Select(x => x.UserId);
        }

        [SetUp]
        public void SetUp()
        {
            CreateMemoryContext();

            repository = new ProjectRepository(provider);

            provider.Projects.Add(newProject);
            provider.Save();
        }
        #endregion

        #region successful test
        [Test]
        public void ShouldDisableWorkersSuccessfully()
        {
            repository.DisableWorkersInProject(projectIdRequest, userIdsRequest);

            var project = provider.Projects
                .FirstOrDefault(p => p.Id == projectIdRequest);

            Assert.Multiple(() =>
            {
                foreach (var user in project.Users)
                {
                    user.Project = null;

                    Assert.IsFalse(user.IsActive);

                }

                for (int i = 0; i < project.Users.Count; i++)
                {

                }
            });
        }
        #endregion

        #region fail tests
        [Test]
        public void ShouldThrowNullReferenceExceptionWhenProjectIdNotFound()
        {
            Assert.That(() => repository.DisableWorkersInProject(Guid.NewGuid(), userIdsRequest),
                Throws.InstanceOf<NotFoundException>());
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenWorkerIdNotFound()
        {
            var randomUsers = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            Assert.That(() => repository.DisableWorkersInProject(projectIdRequest, randomUsers),
                Throws.InstanceOf<NotFoundException>());
        }
        #endregion

        [TearDown]
        public void CleanDbMemory()
        {
            if (provider.IsInMemory())
            {
                provider.EnsureDeleted();
            }
        }
    }
}
