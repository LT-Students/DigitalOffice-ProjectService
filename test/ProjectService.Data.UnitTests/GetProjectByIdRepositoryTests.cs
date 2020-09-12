using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Repositories
{
    public class GetProjectByIdRepositoryTests
    {
        private IDataProvider provider;
        private IProjectRepository repository;

        private DbProject dbProject;

        [SetUp]
        public void SetUp()
        {
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                                    .UseInMemoryDatabase("InMemoryDatabase")
                                    .Options;

            provider = new ProjectServiceDbContext(dbOptions);
            repository = new ProjectRepository(provider);

            dbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "Project"
            };

            provider.Projects.Add(dbProject);
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
        public void ShouldThrowExceptionWhenProjectDoesNotExist()
        {
            Assert.Throws<Exception>(() => repository.GetProjectInfoById(Guid.NewGuid()));
            Assert.That(provider.Projects, Is.EquivalentTo(new List<DbProject> {dbProject}));
        }

        [Test]
        public void ShouldReturnProjectInfo()
        {
            var result = repository.GetProjectInfoById(dbProject.Id);

            var expected = new DbProject
            {
                Id = dbProject.Id,
                Name = dbProject.Name
            };

            SerializerAssert.AreEqual(expected, result);
            Assert.That(provider.Projects, Is.EquivalentTo(new List<DbProject> {dbProject}));
        }
    }
}