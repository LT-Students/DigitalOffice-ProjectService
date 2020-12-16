using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Repositories
{
    public class GetProjectUserByIdRepositoryTests
    {
        private IDataProvider provider;
        private IProjectRepository repository;

        private DbProjectUser dbProjectUser;

        [SetUp]
        public void SetUp()
        {
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                                    .UseInMemoryDatabase("InMemoryDatabase")
                                    .Options;

            provider = new ProjectServiceDbContext(dbOptions);
            repository = new ProjectRepository(provider);

            dbProjectUser = new DbProjectUser
            {
                Id = Guid.NewGuid()
            };

            provider.ProjectsUsers.Add(dbProjectUser);
            provider.Save();
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
            Assert.Throws<NotFoundException>(() => repository.GetProject(Guid.NewGuid()));
            Assert.That(provider.ProjectsUsers, Is.EquivalentTo(new List<DbProjectUser> { dbProjectUser }));
        }

        [Test]
        public void ShouldReturnProjectInfo()
        {
            var result = repository.GetProject(dbProjectUser.Id);

            var expected = new DbProject
            {
                Id = dbProjectUser.Id,
            };

            SerializerAssert.AreEqual(expected, result);
            Assert.That(provider.ProjectsUsers, Is.EquivalentTo(new List<DbProjectUser> { dbProjectUser }));
        }
    }
}