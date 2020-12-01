using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
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
        private List<ProjectUserRequest> projectUsersIds;
        private List<DbProjectUser> dbProjectUsers;
        private ProjectExpandedRequest workersIdsInProjectRequest;
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

            projectUsersIds = new List<ProjectUserRequest>();

            dbProjectUsers = new List<DbProjectUser>();
            workersIdsInProjectRequest = new ProjectExpandedRequest();
            workersIdsInProjectRequest.Project = new Project();

            for (int i = 0; i < 3; i++)
            {
                var dbProjectUser = new DbProjectUser
                {
                    ProjectId = projectId,
                    UserId = Guid.NewGuid(),
                    AddedOn = DateTime.Today,
                    RemovedOn = DateTime.Today,
                    IsActive = true
                };

                dbProjectUsers.Add(dbProjectUser);

                projectUsersIds.Add(new ProjectUserRequest
                {
                    User = new UserRequest
                    {
                        Id = dbProjectUser.UserId
                    }
                });
            }

            newProject.Users = dbProjectUsers;
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
            workersIdsInProjectRequest.Project.Id = newProject.Id;
            workersIdsInProjectRequest.Users = projectUsersIds;

            repository.DisableWorkersInProject(workersIdsInProjectRequest);

            var project = provider.Projects
                .FirstOrDefault(p => p.Id == workersIdsInProjectRequest.Project.Id);

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
            workersIdsInProjectRequest.Project.Id = Guid.NewGuid();

            Assert.That(() => repository.DisableWorkersInProject(workersIdsInProjectRequest),
                Throws.InstanceOf<NullReferenceException>().And
                .Message.EqualTo("Project with this Id does not exist."));
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenWorkerIdNotFound()
        {
            workersIdsInProjectRequest.Project.Id = newProject.Id;
            workersIdsInProjectRequest.Users = new List<ProjectUserRequest>
            {
                new ProjectUserRequest
                {
                    User = new UserRequest
                    {
                        Id = Guid.NewGuid()
                    }
                }
            };

            Assert.That(() => repository.DisableWorkersInProject(workersIdsInProjectRequest),
                Throws.InstanceOf<NullReferenceException>().And
                .Message.EqualTo("Worker with this Id does not exist."));
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
