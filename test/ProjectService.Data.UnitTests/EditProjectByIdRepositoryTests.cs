using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
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

        private Guid departmentId;

        private DbProject dbProject;
        private DbProject editProject;
        private DbContextOptions<ProjectServiceDbContext> dbOptionsProjectService;

        [SetUp]
        public void SetUp()
        {
            dbOptionsProjectService = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "ProjectServiceTest")
                .Options;

            provider = new ProjectServiceDbContext(dbOptionsProjectService);
            repository = new ProjectRepository(provider);

            departmentId = Guid.NewGuid();

            dbProject = new DbProject
            {
                Name = "A test name",
                DepartmentId = departmentId,
                Description = "Description",
                IsActive = true,
                Deferred = false
            };

            editProject = new DbProject
            {
                Name = "Is different",
                DepartmentId = Guid.NewGuid(),
                Description = "Is different too",
                IsActive = false,
                Deferred = false
            };
        }

        [Test]
        public void ShouldReturnProjectGuidWhenProjectIsEdited()
        {
            DbProject existingProject;
            DbProject editedProject;

            dbProject.Id = Guid.NewGuid();
            editProject.Id = dbProject.Id;

            provider.Projects.Add(dbProject);
            provider.SaveModelsChanges();

            provider.MakeEntityDetached(dbProject);
            provider.SaveModelsChanges();

            existingProject = provider.Projects
                .AsNoTracking()
                .SingleOrDefault(p => p.Id == dbProject.Id);

            repository.EditProjectById(editProject);
            provider.MakeEntityDetached(editProject);
            provider.SaveModelsChanges();

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
            this.dbProject.Id = Guid.NewGuid();

            editProject.Id = dbProject.Id;
            editProject.Name = dbProject.Name;
            editProject.IsActive = dbProject.IsActive;
            editProject.Description = dbProject.Description;
            editProject.Deferred = dbProject.Deferred;
            editProject.DepartmentId = dbProject.DepartmentId;

            provider.Projects.Add(dbProject);
            provider.SaveModelsChanges();
            provider.MakeEntityDetached(dbProject);
            provider.SaveModelsChanges();

            var existingProject = provider.Projects
                .AsNoTracking()
                .SingleOrDefault(p => p.Id == dbProject.Id);
            Assert.IsNotNull(existingProject);

            repository.EditProjectById(editProject);
            provider.MakeEntityDetached(editProject);
            provider.SaveModelsChanges();

            var editedProject = provider.Projects
                .AsNoTracking()
                .SingleOrDefault(p => p.Id == editProject.Id);
            Assert.IsNotNull(editedProject);

            Assert.AreEqual(existingProject.Id, editedProject.Id);
            SerializerAssert.AreEqual(existingProject, editedProject);
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenNoGuidIsPassedIn()
        {
            Assert.Throws<NullReferenceException>(() => repository.EditProjectById(editProject));
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenGuidIsNull()
        {
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