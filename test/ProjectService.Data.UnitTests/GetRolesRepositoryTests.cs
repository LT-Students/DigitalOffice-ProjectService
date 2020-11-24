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
    class GetRolesRepositoryTests
    {
        private IDataProvider provider;
        private IRoleRepository repository;

        private List<DbRole> dbRoles;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            dbRoles = new List<DbRole>();

            for (int i = 0; i < 3; i++)
            {

                dbRoles.Add(
                    new DbRole
                    {
                        Id = Guid.NewGuid(),
                        Name = $"Role {i}",
                        Description = $"Role {i} description"
                    });
            }
        }

        [SetUp]
        public void SetUp()
        {
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                                    .UseInMemoryDatabase("ProjectServiceTest")
                                    .Options;

            provider = new ProjectServiceDbContext(dbOptions);
            repository = new RoleRepository(provider);

            foreach (DbRole dbRole in dbRoles)
            {
                provider.Roles.Add(dbRole);
            }

            provider.Save();
        }

        [TearDown]
        public void CleanDbMemory()
        {
            if (provider.IsInMemory())
            {
                provider.EnsureDeleted();
            }
        }

        [Test]
        public void ShouldReturnAllRoles()
        {
            var result = repository.GetRoles(0, 3);

            var expected = new List<DbRole>();

            foreach (DbRole dbRole in dbRoles)
            {
                expected.Add(
                    new DbRole
                    {
                        Id = dbRole.Id,
                        Name = dbRole.Name,
                        Description = dbRole.Description

                    });
            }

            SerializerAssert.AreEqual(expected, result);
            Assert.That(provider.Roles, Is.EquivalentTo(dbRoles));
        }

        [Test]
        public void ShouldReturnAllRolesWhenRequestedMoreThanExists()
        {
            var result = repository.GetRoles(0, 4);

            var expected = new List<DbRole>();

            foreach (DbRole dbRole in dbRoles)
            {
                expected.Add(
                    new DbRole
                    {
                        Id = dbRole.Id,
                        Name = dbRole.Name,
                        Description = dbRole.Description

                    });
            }

            SerializerAssert.AreEqual(expected, result);
            Assert.That(provider.Roles, Is.EquivalentTo(dbRoles));
        }

        [Test]
        public void ShouldReturnTwoLastRoles()
        {
            var result = repository.GetRoles(1, 2);

            var expected = new List<DbRole>();

            for (int i = 1; i < 3; i++)
            {
                expected.Add(
                    new DbRole
                    {
                        Id = dbRoles[i].Id,
                        Name = dbRoles[i].Name,
                        Description = dbRoles[i].Description

                    });
            }

            SerializerAssert.AreEqual(expected, result);
            Assert.That(provider.Roles, Is.EquivalentTo(dbRoles));
        }

        [Test]
        public void ShouldReturnEmptyWhenSkipMoreThanExists()
        {
            var result = repository.GetRoles(3, 3);

            var expected = new List<DbRole>();

            SerializerAssert.AreEqual(expected, result);
            Assert.That(provider.Roles, Is.EquivalentTo(dbRoles));
        }
    }
}
