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
    public class EditProjectByIdRepositoryTests
    {
        private IDataProvider provider;
        private ProjectRepository repository;

        private DbProject dbProject;
        private DbProject editProject;
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

            dbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "A test name",
                ShortName = "Test",
                DepartmentId = Guid.NewGuid(),
                Description = "Description",
                IsActive = true
            };

            editProject = new DbProject
            {
                Id = dbProject.Id,
                Name = "Is different",
                ShortName = "Test",
                DepartmentId = Guid.NewGuid(),
                Description = "Is different too",
                IsActive = false
            };

            provider.Projects.Add(dbProject);
            provider.Save();
            provider.MakeEntityDetached(dbProject);
            provider.Save();
        }

        [Test]
        public void ShouldReturnProjectGuidWhenProjectIsEdited()
        {
            DbProject existingProject;
            DbProject editedProject;

            existingProject = provider.Projects
                .AsNoTracking()
                .SingleOrDefault(p => p.Id == dbProject.Id);

            repository.EditProjectById(editProject);
            provider.MakeEntityDetached(editProject);
            provider.Save();

            editedProject = provider.Projects
                .AsNoTracking()
                .SingleOrDefault(p => p.Id == editProject.Id);

            Assert.IsNotNull(existingProject);
            Assert.IsNotNull(editedProject);
            Assert.AreEqual(existingProject.Id, editedProject.Id);
            SerializerAssert.AreNotEqual(existingProject, editedProject);
            SerializerAssert.AreEqual(editedProject, editProject);
        }

        [Test]
        public void ShouldThrowNoExceptionsWhenNoChangesMadeToDbProject()
        {
            editProject.Name = dbProject.Name;
            editProject.IsActive = dbProject.IsActive;
            editProject.Description = dbProject.Description;
            editProject.DepartmentId = dbProject.DepartmentId;

            var existingProject = provider.Projects
                .AsNoTracking()
                .SingleOrDefault(p => p.Id == dbProject.Id);
            Assert.IsNotNull(existingProject);

            repository.EditProjectById(editProject);
            provider.MakeEntityDetached(editProject);
            provider.Save();

            var editedProject = provider.Projects
                .AsNoTracking()
                .SingleOrDefault(p => p.Id == editProject.Id);
            Assert.IsNotNull(editedProject);

            Assert.AreEqual(existingProject.Id, editedProject.Id);
            SerializerAssert.AreEqual(existingProject, editedProject);
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenProjectUserIsNotFound()
        {
            editProject.Id = Guid.Empty;
            Assert.Throws<NullReferenceException>(() => repository.EditProjectById(editProject));
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