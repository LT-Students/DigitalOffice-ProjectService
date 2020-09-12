﻿using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
    class CreateNewProjectRepositoryTests
    {
        private IDataProvider provider;
        private IProjectRepository repository;

        private ProjectServiceDbContext dbContext;

        private DbProject newProject;

        [SetUp]
        public void SetUp()
        {
            var dbOptionsProjectService = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase("ProjectServiceTest")
                .Options;

            dbContext = new ProjectServiceDbContext(dbOptionsProjectService);
            repository = new ProjectRepository(dbContext);

            newProject = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "DigitalOffice",
                DepartmentId = Guid.NewGuid(),
                Description = "New project for Lanit-Tercom",
                Deferred = false,
                IsActive = true
            };

            repository.CreateNewProject(newProject);
        }

        [Test]
        public void ShouldAddNewProjectWhenTheNameWasRepeated()
        {
            var newProjectWithRepeatedName = newProject;
            newProjectWithRepeatedName.Id = Guid.NewGuid();

            Assert.That(repository.CreateNewProject(newProject), Is.EqualTo(newProjectWithRepeatedName.Id));
            SerializerAssert.AreEqual(newProjectWithRepeatedName, dbContext.Projects.FirstOrDefault(project => project.Id == newProjectWithRepeatedName.Id));
        }

        [Test]
        public void ShouldAddNewProjectToDb()
        {
            newProject.Name = "Any name";
            newProject.Id = Guid.NewGuid();

            Assert.AreEqual(newProject.Id, repository.CreateNewProject(newProject));
            Assert.That(dbContext.Projects.Find(newProject.Id), Is.EqualTo(newProject));
        }

        [TearDown]
        public void CleanMemoryDb()
        {
            if (dbContext.Database.IsInMemory())
            {
                dbContext.Database.EnsureDeleted();
            }
        }
    }
}