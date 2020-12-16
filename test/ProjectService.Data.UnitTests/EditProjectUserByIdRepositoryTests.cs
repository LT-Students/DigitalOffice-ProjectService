using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Repositories
{
    public class EditProjectUserByIdRepositoryTests
    {
        private IDataProvider provider;
        private ProjectRepository repository;

        private DbProjectUser dbProjectUser;
        private DbProjectUser editProjectUser;
        private DbContextOptions<ProjectServiceDbContext> dbOptionsProjectService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            dbOptionsProjectService = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "ProjectServiceTest")
                .Options;
        }

        [SetUp]
        public void SetUp()
        {
            provider = new ProjectServiceDbContext(dbOptionsProjectService);
            repository = new ProjectRepository(provider);

            dbProjectUser = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                AddedOn = new DateTime(2020, 12, 23),
                RemovedOn = new DateTime(2021, 12, 23),
                IsActive = true
            };

            editProjectUser = new DbProjectUser
            {
                Id = dbProjectUser.Id,
                RoleId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                AddedOn = new DateTime(2020, 1, 23),
                RemovedOn = new DateTime(2021, 1, 23),
                IsActive = false
            };

            provider.ProjectsUsers.Add(dbProjectUser);
            provider.Save();
            provider.MakeEntityDetached(dbProjectUser);
            provider.Save();
        }

        [Test]
        public void ShouldReturnProjectUserGuidWhenProjectIsEdited()
        {
            DbProjectUser existingProjectUser;
            DbProjectUser editedProjectUser;

            existingProjectUser = provider.ProjectsUsers
                .AsNoTracking()
                .SingleOrDefault(p => p.Id == dbProjectUser.Id);

            repository.EditProjectUserById(editProjectUser);
            provider.MakeEntityDetached(editProjectUser);
            provider.Save();

            editedProjectUser = provider.ProjectsUsers
                .AsNoTracking()
                .SingleOrDefault(p => p.Id == editProjectUser.Id);

            Assert.IsNotNull(existingProjectUser);
            Assert.IsNotNull(editedProjectUser);
            Assert.AreEqual(existingProjectUser.Id, editedProjectUser.Id);
            SerializerAssert.AreNotEqual(existingProjectUser, editedProjectUser);
            SerializerAssert.AreEqual(editedProjectUser, editProjectUser);
        }

        [Test]
        public void ShouldThrowNoExceptionsWhenNoChangesMadeToDbProjectUser()
        {
            editProjectUser.RoleId = dbProjectUser.RoleId;
            editProjectUser.ProjectId = dbProjectUser.ProjectId;
            editProjectUser.UserId = dbProjectUser.UserId;
            editProjectUser.AddedOn = dbProjectUser.AddedOn;
            editProjectUser.RemovedOn = dbProjectUser.RemovedOn;
            editProjectUser.IsActive = dbProjectUser.IsActive;

            var existingProjectUser = provider.Projects
                .AsNoTracking()
                .SingleOrDefault(p => p.Id == dbProjectUser.Id);
            Assert.IsNotNull(existingProjectUser);

            repository.EditProjectUserById(editProjectUser);
            provider.MakeEntityDetached(editProjectUser);
            provider.Save();

            var editedProjectUser = provider.ProjectsUsers
                .AsNoTracking()
                .SingleOrDefault(p => p.Id == editProjectUser.Id);
            Assert.IsNotNull(editedProjectUser);

            Assert.AreEqual(existingProjectUser.Id, editedProjectUser.Id);
            SerializerAssert.AreEqual(existingProjectUser, editedProjectUser);
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenProjectUserIsNotFound()
        {
            editProjectUser.Id = Guid.Empty;
            Assert.Throws<NullReferenceException>(() => repository.EditProjectUserById(editProjectUser));
        }

        [TearDown]
        public void CleanMemoryDb()
        {
            if (provider.IsInMemory())
            {
                provider.EnsureDeleted();
            }
        }
    }
}