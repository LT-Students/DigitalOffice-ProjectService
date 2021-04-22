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
    internal class EditProjectByIdRepositoryTests
    {
        /*private IDataProvider provider;
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
                ShortName = "Test",
                DepartmentId = departmentId,
                Description = "Description"
            };

            editProject = new DbProject
            {
                Name = "Is different",
                ShortName = "Test",
                DepartmentId = Guid.NewGuid(),
                Description = "Is different too"
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
            provider.Save();

            provider.MakeEntityDetached(dbProject);
            provider.Save();

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
            this.dbProject.Id = Guid.NewGuid();

            editProject.Id = dbProject.Id;
            editProject.Name = dbProject.Name;
            editProject.Description = dbProject.Description;
            editProject.DepartmentId = dbProject.DepartmentId;

            provider.Projects.Add(dbProject);
            provider.Save();
            provider.MakeEntityDetached(dbProject);
            provider.Save();

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
        }*/
    }
}